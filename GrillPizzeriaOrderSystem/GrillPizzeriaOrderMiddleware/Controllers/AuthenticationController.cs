using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DTO.Authentication;
using GrillPizzeriaOrderMiddleware.DatabaseContexts;
using GrillPizzeriaOrderMiddleware.Services.AlgoritamCryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace GrillPizzeriaOrderMiddleware.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly GrillPizzaDatabaseContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationController(GrillPizzaDatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.User
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u =>
                    u.Username == loginUser.Username.Trim() &&
                    u.PasswordHash == Hashing.sha256(loginUser.Password));
                
            if (user == null)
                return Unauthorized("Invalid credentials.");

            string token = GenerateJwtToken(user);
            _context.Log.Add(new Log
            {
                Level = "Info",
                Message = $"New user logged in by id={user.Id}"
            });
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.User.Any(u => u.Username == registerUser.Username.Trim()))
                return BadRequest("Username is already being used.");

            if (_context.User.Any(u => u.Email == registerUser.Email.Trim()))
                return BadRequest("Email is already being used.");

            var user = new User
            {
                Username = registerUser.Username.Trim(),
                PasswordHash = Hashing.sha256(registerUser.Password),
                Email = registerUser.Email.Trim(),
                FirstName = registerUser.FirstName.Trim(),
                LastName = registerUser.LastName.Trim(),
                Phone = registerUser.Phone.Trim(),
                RoleId = _context.Role.First(r => r.Name == "user").Id
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            _context.Log.Add(new Log
            {
                Level = "Info",
                Message = $"User logged-in by id={user.Id}"
            });
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpPost("changepassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePassword)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? sid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (sid == null || !int.TryParse(sid, out var userId))
                return Unauthorized();

            var user = await _context.User.FindAsync(userId);
            var oldPassword = Hashing.sha256(changePassword.OldPassword);
            var newPassword = Hashing.sha256(changePassword.NewPassword);

            if (user == null)
                return NotFound("User not found.");

            if (user.PasswordHash != oldPassword)
                return BadRequest("Old password is incorrect.");

            if (oldPassword == newPassword)
                return BadRequest("Old and new passwords are the same");

            user.PasswordHash = newPassword;
            await _context.SaveChangesAsync();

            _context.Log.Add(new Log
            {
                Level = "Info",
                Message = $"User by id={user.Id} changed password."
            });
            await _context.SaveChangesAsync();

            return Ok("Password successfully changed");
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var keyBytes = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,           user.Username),
                new Claim(ClaimTypes.Email,          user.Email),
                new Claim(ClaimTypes.Role,           user.Role.Name)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(5),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
