using Microsoft.AspNetCore.Mvc;
using Moduls;

namespace GrillPizzeriaOrderMiddleware.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly YourDbContext _context;

        public AuthenticationController(YourDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
        {
            if (_context.Users.Any(u => u.Username == dto.Username))
                return BadRequest("Username already exists.");

            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Email already exists.");

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = Sha1Hasher.Hash(dto.Password),
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                RoleID = _context.Roles.First(r => r.Name == "user").Id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });
        }
    }
}
