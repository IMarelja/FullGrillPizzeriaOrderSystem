using System.Security.Claims;
using AutoMapper;
using DTO.User;
using GrillPizzeriaOrderMiddleware.DatabaseContexts;
using GrillPizzeriaOrderMiddleware.Services.AppLogging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models;

namespace GrillPizzeriaOrderMiddleware.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly GrillPizzaDatabaseContext _context;
        private readonly IMapper _mapper;

        public UserController(GrillPizzaDatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserReadDto>> GetMe([FromServices] IAppLogger log)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                    return Unauthorized("Invalid token.");

                var user = await _context.User
                    .AsNoTracking()
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null) return NotFound();

                var dto = _mapper.Map<UserReadDto>(user);
                return Ok(dto);
            }
            catch (DbUpdateException ex)
            {
                await log.Error($"User.GetMe database error: {ex.InnerException?.Message ?? ex.Message}");
                return StatusCode(500, "Database execution error. Operation could not be completed.");
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "User.GetMe, database connection error: " + ex.Message);
            }
            catch (SqlException ex)
            {
                await log.Error($"User.GetMe SQL error: {ex.Message}");
                return StatusCode(500, "SQL error occurred: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"User.GetMe failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAll(
            [FromServices] IAppLogger log,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50
        )

        {
            try
            {
                if (take is < 1 or > 200) take = 50;

                var users = await _context.User
                    .AsNoTracking()
                    .Include(u => u.Role)
                    .OrderBy(u => u.Id)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();

                var dtos = _mapper.Map<IEnumerable<UserReadDto>>(users);
                return Ok(dtos);
            }
            catch (DbUpdateException ex)
            {
                await log.Error($"User.GetAll database error: {ex.InnerException?.Message ?? ex.Message}");
                return StatusCode(500, "Database execution error. Operation could not be completed.");
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "User.GetAll, database connection error: " + ex.Message);
            }
            catch (SqlException ex)
            {
                await log.Error($"User.GetAll SQL error: {ex.Message}");
                return StatusCode(500, "SQL error occurred: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"User.GetAll failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMe([FromBody] UserUpdateDto dto, [FromServices] IAppLogger log)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                    return Unauthorized("Invalid token.");

                var user = await _context.User
                    .Include(u => u.Role) // not necessary for update, but sometimes useful for logs
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return NotFound();

                var emailInUse = await _context.User.AnyAsync(u => u.Email == dto.email && u.Id != userId);
                if (emailInUse)
                    return BadRequest("Email is already in use.");

                // Map only allowed fields (your mapping profile should ignore restricted fields)
                _mapper.Map(dto, user);

                await _context.SaveChangesAsync();

                return Ok(_mapper.Map<UserReadDto>(user));
            }
            catch (DbUpdateException ex)
            {
                await log.Error($"User.UpdateMe database error: {ex.InnerException?.Message ?? ex.Message}");
                return StatusCode(500, "Database execution error. Operation could not be completed.");
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "User.UpdateMe, database connection error: " + ex.Message);
            }
            catch (SqlException ex)
            {
                await log.Error($"User.UpdateMe SQL error: {ex.Message}");
                return StatusCode(500, "SQL error occurred: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"User.UpdateMe failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
