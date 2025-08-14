using AutoMapper;
using DTO.Order;
using GrillPizzeriaOrderMiddleware.DatabaseContexts;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetAll()
        {
            var orders = await _context.Order
                .Include(o => o.User)
                .Include(o => o.OrderFoods).ThenInclude(of => of.Food)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<OrderReadDto>>(orders));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderReadDto>> GetById(int id)
        {
            var order = await _context.Order
                .Include(o => o.User)
                .Include(o => o.OrderFoods).ThenInclude(of => of.Food)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            return Ok(_mapper.Map<OrderReadDto>(order));
        }

        [HttpPost]
        public async Task<ActionResult<OrderReadDto>> Create([FromBody] OrderCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Validate user
            var userExists = await _context.User.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists) return BadRequest("Invalid UserId.");

            if (dto.FoodIds == null || dto.FoodIds.Count == 0)
                return BadRequest("Order must contain at least one food item.");

            // Validate foods
            var validFoodIds = await _context.Food
                .Where(f => dto.FoodIds.Contains(f.Id))
                .Select(f => f.Id)
                .ToListAsync();

            if (validFoodIds.Count != dto.FoodIds.Count)
                return BadRequest("One or more provided FoodIds are invalid.");

            var order = new Order
            {
                UserId = dto.UserId,
                OrderDate = DateTime.UtcNow
            };

            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            foreach (var fid in validFoodIds)
            {
                _context.OrderFood.Add(new OrderFood { OrderId = order.Id, FoodId = fid });
            }

            await _context.SaveChangesAsync();

            // Reload with includes for return
            var created = await _context.Order
                .Include(o => o.User)
                .Include(o => o.OrderFoods).ThenInclude(of => of.Food)
                .FirstAsync(o => o.Id == order.Id);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<OrderReadDto>(created));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Order
                .Include(o => o.OrderFoods)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            // Remove bridge rows first (if cascade not set)
            _context.OrderFood.RemoveRange(order.OrderFoods);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
