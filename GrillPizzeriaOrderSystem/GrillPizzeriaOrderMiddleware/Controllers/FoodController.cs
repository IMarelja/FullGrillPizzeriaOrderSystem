using AutoMapper;
using DTO.Food;
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
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<FoodReadDto>>> GetAll([FromServices] IAppLogger log)
        {
            try
            {
                var items = await _context.Food
                        .Include(f => f.FoodCategory)
                        .Include(f => f.FoodAllergens).ThenInclude(fa => fa.Allergen)
                        .OrderBy(f => f.Name)
                        .ToListAsync();

                return Ok(_mapper.Map<IEnumerable<FoodReadDto>>(items));
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "Food.GetAll, database connection error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"Food.GetAll failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/food/5
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<FoodReadDto>> GetById(int id, [FromServices] IAppLogger log)
        {
            try
            {
                var entity = await _context.Food
                        .Include(f => f.FoodCategory)
                        .Include(f => f.FoodAllergens).ThenInclude(fa => fa.Allergen)
                        .FirstOrDefaultAsync(f => f.Id == id);

                if (entity == null) return NotFound();
                return Ok(_mapper.Map<FoodReadDto>(entity));
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "Food.GetById, database connection error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"Food.GetById failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/food/search?q=margherita&categoryId=2&page=1&pageSize=10
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Search(
            [FromServices] IAppLogger log,
            [FromQuery] string? q,
            [FromQuery] int? categoryId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            try
            {
                if (page < 1 || pageSize < 1 || pageSize > 100)
                {
                    await log.Error($"Food.Search failed: Invalid paging parameters. page={page}, pageSize={pageSize}");
                    return BadRequest("Invalid paging parameters.");
                }

                // Base query
                var query = _context.Food
                    .AsNoTracking()
                    .Include(f => f.FoodCategory)
                    .Include(f => f.FoodAllergens).ThenInclude(fa => fa.Allergen)
                    .AsQueryable();

                // Search (case-insensitive LIKE)
                if (!string.IsNullOrWhiteSpace(q))
                {
                    var pattern = $"%{q.Trim()}%";
                    query = query.Where(f =>
                        EF.Functions.Like(f.Name, pattern) ||
                        EF.Functions.Like(f.Description!, pattern));
                }

                // Category filter
                if (categoryId.HasValue)
                {
                    query = query.Where(f => f.FoodCategoryId == categoryId.Value);
                }

                // Count first
                var total = await query.CountAsync();

                // Compute totalPages (0 when no results)
                var totalPages = (int)Math.Ceiling(total / (double)pageSize);

                // Deterministic ordering for stable pagination
                query = query.OrderBy(f => f.Name).ThenBy(f => f.Id);

                // Page slice
                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var dtos = _mapper.Map<IEnumerable<FoodReadDto>>(items);

                await log.Information(
                    $"Food.Search filters (q={q ?? "null"}, categoryId={categoryId?.ToString() ?? "null"}), page={page}, pageSize={pageSize}, total={total}, totalPages={totalPages}");

                return Ok(new
                {
                    total,
                    currentPage = page,
                    currentPageSize = dtos.Count(),
                    pageSize,
                    totalPages,
                    data = dtos
                });
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "Food.Search, database connection error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"Food.Search failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        // POST: api/food
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<FoodReadDto>> Create(
            [FromBody] FoodCreateDto dto,
            [FromServices] IAppLogger log)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await log.Error("Food.Create failed: Bad Request");
                    return BadRequest(ModelState);
                }

                var nameTaken = await _context.Food.AnyAsync(f => f.Name == dto.Name);
                if (nameTaken)
                {
                    await log.Error($"Food.Create failed: Conflict with {dto.Name}");
                    return BadRequest("Food with the same name already exists.");
                }

                // Validate FK
                var categoryExists = await _context.FoodCategory.AnyAsync(c => c.Id == dto.FoodCategoryId);
                if (!categoryExists)
                {
                    await log.Error("Food.Create failed: Invalid FoodCategoryId");
                    return BadRequest("Invalid FoodCategoryId.");
                }

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

                await log.Information($"Food.Create success: id={entity.Id}");
                return CreatedAtAction(nameof(GetById), new { id = entity.Id }, _mapper.Map<FoodReadDto>(read));
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "Food.Search, database connection error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"Food.Search failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // PUT: api/food/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] FoodCreateDto dto,
            [FromServices] IAppLogger log)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await log.Error("Food.Update failed: Bad Request");
                    return BadRequest(ModelState);
                }

                var entity = await _context.Food
                    .Include(f => f.FoodAllergens)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (entity == null) return NotFound();

                if (!string.Equals(entity.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var nameTaken = await _context.Food.AnyAsync(f => f.Name == dto.Name);
                    if (nameTaken)
                    {
                        await log.Error($"Food.Update failed: Conflict with {dto.Name}");
                        return BadRequest("Food with the same name already exists.");
                    }
                }

                if (!await _context.FoodCategory.AnyAsync(c => c.Id == dto.FoodCategoryId))
                {
                    await log.Error("Food.Update failed: Invalid FoodCategoryId");
                    return BadRequest("Invalid FoodCategoryId.");
                }

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

                await log.Information($"Food.Update success: id={entity.Id}");
                return Ok($"Successful update of {entity.Name} in {entity.Id}");
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "Food.Update, database connection error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"Food.Update failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // DELETE: api/food/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id, [FromServices] IAppLogger log)
        {
            try
            {
                var entity = await _context.Food.FindAsync(id);
                if (entity == null) return NotFound();

                _context.Food.Remove(entity);
                await _context.SaveChangesAsync();

                await log.Information($"Food.Delete success: id={id}");
                return Ok($"Successfully deletion of {entity.Name}");
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "Food.Delete, database connection error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"Food.Delete failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
