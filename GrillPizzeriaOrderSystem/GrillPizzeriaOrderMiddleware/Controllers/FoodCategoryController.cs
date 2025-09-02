using AutoMapper;
using DTO.FoodCategory;
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
    public class FoodCategoryController : ControllerBase
    {
        private readonly GrillPizzaDatabaseContext _context;
        private readonly IMapper _mapper;

        public FoodCategoryController(GrillPizzaDatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Create
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<FoodCategoryReadDto>> Create(
            [FromBody] FoodCategoryCreateDto createDto,
            [FromServices] IAppLogger log)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await log.Error("FoodCategory.Create failed: Bad Request");
                    return BadRequest(ModelState);
                }

                // Duplicate name check (mirror Allergen style)
                var exists = await _context.FoodCategory.AnyAsync(c => c.Name == createDto.Name);
                if (exists)
                {
                    await log.Error($"FoodCategory.Create failed: Conflict with {createDto.Name}");
                    return BadRequest("Category with the same name already exists.");
                }

                var entity = _mapper.Map<FoodCategory>(createDto);
                _context.FoodCategory.Add(entity);
                await _context.SaveChangesAsync();

                var readDto = _mapper.Map<FoodCategoryReadDto>(entity);
                await log.Information($"FoodCategory.Create success: id={entity.Id}");
                return CreatedAtAction(nameof(Get), new { id = readDto.Id }, readDto);
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "FoodCategory.Create, database connection error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"FoodCategory.Create failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // Read all
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<FoodCategoryReadDto>>> GetAll([FromServices] IAppLogger log)
        {
            try
            {
                var entities = await _context.FoodCategory.ToListAsync();
                var dtos = _mapper.Map<IEnumerable<FoodCategoryReadDto>>(entities);
                return Ok(dtos);
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "FoodCategory.GetAll, database connection error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"FoodCategory.GetAll failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // Read one
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<FoodCategoryReadDto>> Get(int id, [FromServices] IAppLogger log)
        {
            try
            {
                var entity = await _context.FoodCategory.FindAsync(id);
                if (entity == null)
                    return NotFound();

                var dto = _mapper.Map<FoodCategoryReadDto>(entity);
                return Ok(dto);
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "FoodCategory.Get, database connection error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"FoodCategory.Get failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // Update
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] FoodCategoryCreateDto updateDto,
            [FromServices] IAppLogger log)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await log.Error("FoodCategory.Update failed: Bad Request");
                    return BadRequest(ModelState);
                }

                var entity = await _context.FoodCategory.FindAsync(id);
                if (entity == null)
                    return NotFound();

                // If name is changing, ensure uniqueness (mirror Allergen pattern)
                if (!string.Equals(entity.Name, updateDto.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var nameTaken = await _context.FoodCategory.AnyAsync(c => c.Name == updateDto.Name);
                    if (nameTaken)
                    {
                        await log.Error($"FoodCategory.Update failed: Conflict with {updateDto.Name}");
                        return BadRequest("Category with the same name already exists.");
                    }
                }

                _mapper.Map(updateDto, entity);
                await _context.SaveChangesAsync();

                await log.Information($"FoodCategory.Update success: id={entity.Id}");
                return Ok($"Successful update of {entity.Name} in {entity.Id}");
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "FoodCategory.Update, database connection error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"FoodCategory.Update failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // Delete
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id, [FromServices] IAppLogger log)
        {
            try
            {
                var entity = await _context.FoodCategory.FindAsync(id);
                if (entity == null)
                    return NotFound();

                _context.FoodCategory.Remove(entity);
                await _context.SaveChangesAsync();

                await log.Information($"FoodCategory.Delete success: id={id}");
                return Ok($"Successfully deletion of {entity.Name}");
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "FoodCategory.Delete, database connection error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"FoodCategory.Delete failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}