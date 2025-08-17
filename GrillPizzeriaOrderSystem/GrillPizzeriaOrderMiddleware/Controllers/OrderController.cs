using System.Security.Claims;
using AutoMapper;
using DTO.Order;
using GrillPizzeriaOrderMiddleware.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace GrillPizzeriaOrderMiddleware.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly GrillPizzaDatabaseContext _context;
        private readonly IMapper _mapper;

        public OrdersController(GrillPizzaDatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // ADMIN: Get all
        // ─────────────────────────────────────────────────────────────────────────────
        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetAll()
        {
            var orders = await _context.Order
                .Include(o => o.User)
                .Include(o => o.OrderFoods).ThenInclude(of => of.Food)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<OrderReadDto>>(orders));
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // ADMIN: Get by id
        // ─────────────────────────────────────────────────────────────────────────────
        [HttpGet("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<OrderReadDto>> GetById(int id)
        {
            var order = await _context.Order
                .Include(o => o.User)
                .Include(o => o.OrderFoods).ThenInclude(of => of.Food)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            return Ok(_mapper.Map<OrderReadDto>(order));
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // USER: My orders
        // ─────────────────────────────────────────────────────────────────────────────
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetMyOrders()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Invalid token");

            var orders = await _context.Order
                .Where(o => o.UserId == userId)
                .Include(o => o.User)
                .Include(o => o.OrderFoods).ThenInclude(of => of.Food)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<OrderReadDto>>(orders));
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // USER: Create order (with quantities)
        // ─────────────────────────────────────────────────────────────────────────────
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderReadDto>> Create([FromBody] OrderCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Auth user
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Invalid token");

            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest("Order must contain at least one item.");

            // Normalize duplicates & validate quantities (1..100 like your DB CHECK)
            var collapsed = dto.Items
                .GroupBy(i => i.FoodId)
                .Select(g => new { FoodId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToList();

            if (collapsed.Any(x => x.Quantity < 1 || x.Quantity > 100))
                return BadRequest("Quantity must be between 1 and 100.");

            // Validate foods exist
            var foodIds = collapsed.Select(x => x.FoodId).Distinct().ToList();
            var foods = await _context.Food
                .Where(f => foodIds.Contains(f.Id))
                .Select(f => new { f.Id, f.Price })
                .ToListAsync();

            if (foods.Count != foodIds.Count)
                return BadRequest("One or more provided FoodId values are invalid.");

            // Build order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,    // you also have default GETDATE() at DB level
                OrderFoods = collapsed
                    .Select(x => new OrderFood { FoodId = x.FoodId, Quantity = x.Quantity })
                    .ToList()
            };

            // Compute total (decimal!) from current prices
            order.OrderTotalPrice = order.OrderFoods
                .Sum(of => of.Quantity * foods.First(f => f.Id == of.FoodId).Price);

            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            // Reload for return
            var created = await _context.Order
                .Include(o => o.User)
                .Include(o => o.OrderFoods).ThenInclude(of => of.Food)
                .FirstAsync(o => o.Id == order.Id);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<OrderReadDto>(created));
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // USER/ADMIN: Delete order
        // (You have ON DELETE CASCADE for OrderFood; removing the Order is enough.)
        // ─────────────────────────────────────────────────────────────────────────────
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            // Optional: only admin or owner can delete
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdClaim, out int userId);
            bool isAdmin = User.IsInRole("admin");

            var order = await _context.Order
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            if (!isAdmin && order.UserId != userId) return Forbid();

            // With FK ON DELETE CASCADE, this removes children automatically
            _context.Order.Remove(new Order { Id = id });
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
