using System.Security.Claims;
using AutoMapper;
using DTO.Order;
using GrillPizzeriaOrderMiddleware.DatabaseContexts;
using GrillPizzeriaOrderMiddleware.Services.AppLogging;
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

        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetAll([FromServices] IAppLogger log)
        {
            var orders = await _context.Order
                .Include(o => o.User)
                .Include(o => o.OrderFoods).ThenInclude(of => of.Food)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            await log.Information($"Orders.GetAll success: count={orders.Count}");
            return Ok(_mapper.Map<IEnumerable<OrderReadDto>>(orders));
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<OrderReadDto>> GetById(int id, [FromServices] IAppLogger log)
        {
            var order = await _context.Order
                .Include(o => o.User)
                .Include(o => o.OrderFoods).ThenInclude(of => of.Food)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                await log.Error($"Orders.GetById failed: Not Found id={id}");
                return NotFound();
            }

            await log.Information($"Orders.GetById success: id={id}");
            return Ok(_mapper.Map<OrderReadDto>(order));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetMyOrders([FromServices] IAppLogger log)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                await log.Error("Orders.GetMyOrders failed: Invalid token");
                return Unauthorized("Invalid token");
            }

            var orders = await _context.Order
                .Where(o => o.UserId == userId)
                .Include(o => o.User)
                .Include(o => o.OrderFoods).ThenInclude(of => of.Food)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            await log.Information($"Orders.GetMyOrders success: userId={userId}, count={orders.Count}");
            return Ok(_mapper.Map<IEnumerable<OrderReadDto>>(orders));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderReadDto>> Create([FromBody] OrderCreateDto dto, [FromServices] IAppLogger log)
        {
            if (!ModelState.IsValid)
            {
                await log.Error("Orders.Create failed: Bad Request");
                return BadRequest(ModelState);
            }

            // Auth user
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                await log.Error("Orders.Create failed: Invalid token");
                return Unauthorized("Invalid token");
            }

            if (dto.Items == null || dto.Items.Count == 0)
            {
                await log.Error("Orders.Create failed: Empty items");
                return BadRequest("Order must contain at least one item.");
            }

            // Normalize duplicates & validate quantities (1..100)
            var collapsed = dto.Items
                .GroupBy(i => i.FoodId)
                .Select(g => new { FoodId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToList();

            if (collapsed.Any(x => x.Quantity < 1 || x.Quantity > 100))
            {
                await log.Error("Orders.Create failed: Quantity out of range (1..100)");
                return BadRequest("Quantity must be between 1 and 100.");
            }

            // Validate foods exist
            var foodIds = collapsed.Select(x => x.FoodId).Distinct().ToList();
            var foods = await _context.Food
                .Where(f => foodIds.Contains(f.Id))
                .Select(f => new { f.Id, f.Price })
                .ToListAsync();

            if (foods.Count != foodIds.Count)
            {
                await log.Error("Orders.Create failed: Invalid FoodId(s) provided");
                return BadRequest("One or more provided FoodId values are invalid.");
            }

            // Build order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                OrderFoods = collapsed
                    .Select(x => new OrderFood { FoodId = x.FoodId, Quantity = x.Quantity })
                    .ToList()
            };

            // Compute total from current prices
            order.OrderTotalPrice = order.OrderFoods
                .Sum(of => of.Quantity * foods.First(f => f.Id == of.FoodId).Price);

            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            // Reload for return
            var created = await _context.Order
                .Include(o => o.User)
                .Include(o => o.OrderFoods).ThenInclude(of => of.Food)
                .FirstAsync(o => o.Id == order.Id);

            await log.Information($"Orders.Create success: id={order.Id}, items={order.OrderFoods.Count}, total={order.OrderTotalPrice}");
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<OrderReadDto>(created));
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, [FromServices] IAppLogger log)
        {
            // Only admin or owner can delete
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdClaim, out int userId);
            bool isAdmin = User.IsInRole("admin");

            var order = await _context.Order
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                await log.Error($"Orders.Delete failed: Not Found id={id}");
                return NotFound();
            }

            if (!isAdmin && order.UserId != userId)
            {
                await log.Error($"Orders.Delete failed: Forbid userId={userId} for orderId={id}");
                return Forbid();
            }

            _context.Order.Remove(new Order { Id = id });
            await _context.SaveChangesAsync();

            await log.Information($"Orders.Delete success: id={id}, by={(isAdmin ? "admin" : $"user {userId}")}");
            return Ok($"Successfully deleted order {id}");
        }
    }
}
