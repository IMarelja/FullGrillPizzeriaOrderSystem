using AutoMapper;
using DTO.Food;
using GrillPizzeriaOrderMiddleware.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace GrillPizzeriaOrderMiddleware.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodController : ControllerBase
    {
        private readonly GrillPizzaDatabaseContext _context;
        private readonly IMapper _mapper;

        public FoodController(GrillPizzaDatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/food
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FoodReadDto>>> GetAll()
        {
            var items = await _context.Food
                .Include(f => f.FoodCategory)
                .Include(f => f.FoodAllergens).ThenInclude(fa => fa.Allergen)
                .OrderBy(f => f.Name)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<FoodReadDto>>(items));
        }

        // GET: api/food/5
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<FoodReadDto>> GetById(int id)
        {
            var entity = await _context.Food
                .Include(f => f.FoodCategory)
                .Include(f => f.FoodAllergens).ThenInclude(fa => fa.Allergen)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (entity == null) return NotFound();
            return Ok(_mapper.Map<FoodReadDto>(entity));
        }

        // GET: api/food/search?q=margherita&categoryId=2&page=1&pageSize=10
        [HttpGet("search")]
        [Authorize]
        public async Task<ActionResult<object>> Search(
            [FromQuery] string? q,
            [FromQuery] int? categoryId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest("Invalid paging parameters.");

            var query = _context.Food
                .Include(f => f.FoodCategory)
                .Include(f => f.FoodAllergens).ThenInclude(fa => fa.Allergen)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(f => f.Name.Contains(q) || f.Description.Contains(q));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(f => f.FoodCategoryId == categoryId.Value);
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(f => f.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<FoodReadDto>>(items);

            return Ok(new
            {
                page,
                pageSize,
                total,
                data = dtos
            });
        }

        // POST: api/food
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<FoodReadDto>> Create([FromBody] FoodCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var nameTaken = await _context.Food.AnyAsync(f => f.Name == dto.Name);
            if (nameTaken) return BadRequest("Food with the same name already exists.");

            // Validate FK
            var categoryExists = await _context.FoodCategory.AnyAsync(c => c.Id == dto.FoodCategoryId);
            if (!categoryExists) return BadRequest("Invalid FoodCategoryId.");

            var entity = _mapper.Map<Food>(dto);
            _context.Food.Add(entity);
            await _context.SaveChangesAsync();

            // Optional: handle allergens if provided on create
            if (dto.AllergenIds != null && dto.AllergenIds.Count > 0)
            {
                var validAllergenIds = await _context.Allergen
                    .Where(a => dto.AllergenIds.Contains(a.Id))
                    .Select(a => a.Id)
                    .ToListAsync();

                foreach (var aid in validAllergenIds)
                    _context.FoodAllergen.Add(new FoodAllergen { FoodId = entity.Id, AllergenId = aid });

                await _context.SaveChangesAsync();
            }

            var read = await _context.Food
                .Include(f => f.FoodCategory)
                .Include(f => f.FoodAllergens).ThenInclude(fa => fa.Allergen)
                .FirstAsync(f => f.Id == entity.Id);

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, _mapper.Map<FoodReadDto>(read));
        }

        // PUT: api/food/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] FoodUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = await _context.Food
                .Include(f => f.FoodAllergens)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (entity == null) return NotFound();

            if (!string.Equals(entity.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                var nameTaken = await _context.Food.AnyAsync(f => f.Name == dto.Name);
                if (nameTaken) return BadRequest("Food with the same name already exists.");
            }

            if (!await _context.FoodCategory.AnyAsync(c => c.Id == dto.FoodCategoryId))
                return BadRequest("Invalid FoodCategoryId.");

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();

            // Replace allergens if provided
            if (dto.AllergenIds != null)
            {
                // remove current
                _context.FoodAllergen.RemoveRange(entity.FoodAllergens);

                // add new
                var validAllergenIds = await _context.Allergen
                    .Where(a => dto.AllergenIds.Contains(a.Id))
                    .Select(a => a.Id)
                    .ToListAsync();

                foreach (var aid in validAllergenIds)
                    _context.FoodAllergen.Add(new FoodAllergen { FoodId = id, AllergenId = aid });

                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        // DELETE: api/food/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Food.FindAsync(id);
            if (entity == null) return NotFound();

            _context.Food.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
