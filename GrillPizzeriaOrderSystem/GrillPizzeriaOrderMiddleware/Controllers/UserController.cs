using System.Security.Claims;
using AutoMapper;
using DTO.User;
using GrillPizzeriaOrderMiddleware.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GrillPizzeriaOrderMiddleware.Controllers
{
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
        public async Task<ActionResult<UserReadDto>> GetMe()
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

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAll(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
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

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UserUpdateDto dto)
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

            var emailInUse = await _context.User.AnyAsync(u => u.Email == dto.Email && u.Id != userId);
            if (emailInUse) 
                return BadRequest("Email is already in use.");

            // Map only allowed fields (your mapping profile should ignore restricted fields)
            _mapper.Map(dto, user);

            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<UserReadDto>(user));
        }
    }
}
