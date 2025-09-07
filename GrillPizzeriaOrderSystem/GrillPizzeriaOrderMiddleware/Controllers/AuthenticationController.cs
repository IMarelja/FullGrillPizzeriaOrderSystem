using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DTO.Authentication;
using GrillPizzeriaOrderMiddleware.DatabaseContexts;
using GrillPizzeriaOrderMiddleware.Services.AlgoritamCryptography;
using GrillPizzeriaOrderMiddleware.Services.AppLogging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
        public async Task<IActionResult> Login([FromBody] LoginRequest loginUser, [FromServices] IAppLogger log)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _context.User
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u =>
                        u.Username == loginUser.Username.Trim() &&
                        u.PasswordHash == Hashing.sha256(loginUser.Password));

                if (user == null)
                {
                    await log.Error($"Authentication.Login: Invalid credentials from {loginUser.Username}");
                    return Unauthorized("Invalid credentials.");
                }

                string token = GenerateJwtToken(user);
                await log.Information($"Authentication.Login: Logged in user id={user.Id}, role={user.Role.Name}.");
                return Ok(new { token });
            }
            catch (DbUpdateException ex)
            {
                await log.Error($"Authentication.Login database error: {ex.InnerException?.Message ?? ex.Message}");
                return StatusCode(500, "Database execution error. Operation could not be completed.");
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "Authentication.Login, database connection error: " + ex.Message);
            }
            catch (SqlException ex)
            {
                await log.Error($"Authentication.Login SQL error: {ex.Message}");
                return StatusCode(500, "SQL error occurred: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"Authentication.Login failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerUser, [FromServices] IAppLogger log)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                if (_context.User.Any(u => u.Username == registerUser.Username.Trim()))
                    return BadRequest("Username is already being used.");

                if (_context.User.Any(u => u.Email == registerUser.Email.Trim()))
                    return BadRequest("Email is already being used.");

                if (_context.User.Any(u => u.Phone == registerUser.Phone.Trim()))
                    return BadRequest("Phone number is already being used.");

                var user = new User
                {
                    Username = registerUser.Username.Trim(),
                    PasswordHash = Hashing.sha256(registerUser.Password),
                    Email = registerUser.Email.Trim(),
                    FirstName = registerUser.FirstName.Trim(),
                    LastName = registerUser.LastName.Trim(),
                    Phone = registerUser.Phone.Trim(),
                    CreationDate = DateTime.Now,
                    RoleId = _context.Role.First(r => r.Name == "user").Id
                };

                _context.User.Add(user);
                await _context.SaveChangesAsync();

                await log.Information($"Authentication.Register: Registered user id={user.Id}, role={user.Role.Name}.");
                return Ok("User registered successfully.");
            }
            catch (DbUpdateException ex)
            {
                await log.Error($"Authentication.Register database error: {ex.InnerException?.Message ?? ex.Message}");
                return StatusCode(500, "Database execution error. Operation could not be completed.");
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "Authentication.Register, database connection error: " + ex.Message);
            }
            catch (SqlException ex)
            {
                await log.Error($"Authentication.Register SQL error: {ex.Message}");
                return StatusCode(500, "SQL error occurred: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"Authentication.Register failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("changepassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePassword, [FromServices] IAppLogger log)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                string? sid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (sid == null || !int.TryParse(sid, out var userId))
                    return Unauthorized();

                var user = await _context.User.FindAsync(userId);

                if (user == null)
                {
                    await log.Information($"Authentication.ChangePassword: User not found id={user.Id}");
                    return NotFound("User not found.");
                }

                var oldPassword = Hashing.sha256(changePassword.currentPassword);                

                if (user.PasswordHash != oldPassword)
                    return BadRequest("Current password is incorrect.");

                var newPassword = Hashing.sha256(changePassword.newPassword);
                var confirmNewPassword = Hashing.sha256(changePassword.confirmNewPassword);

                if (confirmNewPassword != newPassword)
                    return BadRequest("New passwords aren't the same");

                user.PasswordHash = newPassword;
                await _context.SaveChangesAsync();

                await log.Information($"Authentication.ChangePassword: Changed password user id={user.Id}");

                return Ok("Password successfully changed");
            }
            catch (DbUpdateException ex)
            {
                await log.Error($"Authentication.ChangePassword database error: {ex.InnerException?.Message ?? ex.Message}");
                return StatusCode(500, "Database execution error. Operation could not be completed.");
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "Authentication.ChangePassword, database connection error: " + ex.Message);
            }
            catch (SqlException ex)
            {
                await log.Error($"Authentication.ChangePassword SQL error: {ex.Message}");
                return StatusCode(500, "SQL error occurred: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"Authentication.ChangePassword failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
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
