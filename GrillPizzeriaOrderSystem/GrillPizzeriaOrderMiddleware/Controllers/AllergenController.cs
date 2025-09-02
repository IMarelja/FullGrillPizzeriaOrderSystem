using AutoMapper;
using DTO.Allergen;
using GrillPizzeriaOrderMiddleware.DatabaseContexts;
using GrillPizzeriaOrderMiddleware.Services.AppLogging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models;

namespace GrillPizzeriaOrderMiddleware.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AllergenController : ControllerBase
    {
        private readonly GrillPizzaDatabaseContext _context;
        private readonly IMapper _mapper;

        public AllergenController(GrillPizzaDatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AllergenReadDto>>> GetAll([FromServices] IAppLogger log)
        {
            try { 
                var items = await _context.Allergen
                .OrderBy(a => a.Name)
                .ToListAsync();

                return Ok(_mapper.Map<IEnumerable<AllergenReadDto>>(items));
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "Allergen.GetAll, database connection error: " + ex.Message);
            }
            catch(Exception ex)
            {
                await log.Error($"Allergen.GetAll failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<AllergenReadDto>> GetById(int id, [FromServices] IAppLogger log)
        {
            try
            {
                var entity = await _context.Allergen.FindAsync(id);
                if (entity == null)
                    return NotFound();
                return Ok(_mapper.Map<AllergenReadDto>(entity));
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "Allergen.GetAll, database connection error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"Allergen.GetAll failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<AllergenReadDto>> Create([FromBody] AllergenCreateDto createDto, [FromServices] IAppLogger log)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await log.Error($"Allergen.Create failed: Bad Request");
                    return BadRequest(ModelState);
                }

                var existsAllergen = await _context.Allergen.AnyAsync(a => a.Name == createDto.Name);
                if (existsAllergen)
                {
                    await log.Error($"Allergen.Create failed: Conflict with {createDto.Name}");
                    return BadRequest("Allergen with the same name already exists.");
                }

                var entity = _mapper.Map<Allergen>(createDto);
                _context.Allergen.Add(entity);
                await _context.SaveChangesAsync();

                var read = _mapper.Map<AllergenReadDto>(entity);
                await log.Information($"Allergen.Create success: id={entity.Id}");
                return CreatedAtAction(nameof(GetById), new { id = entity.Id }, read);
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "Allergen.Create, database connection error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"Allergen.Create failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] AllergenCreateDto updateDto, [FromServices] IAppLogger log)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var entity = await _context.Allergen.FindAsync(id);
                if (entity == null)
                    return NotFound();

                if (!string.Equals(entity.Name, updateDto.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var nameTaken = await _context.Allergen.AnyAsync(a => a.Name == updateDto.Name);
                    if (nameTaken)
                    {
                        await log.Error($"Allergen.Create failed: Conflict with {updateDto.Name}");
                        return BadRequest("Allergen with the same name already exists.");
                    }
                }

                _mapper.Map(updateDto, entity);
                await _context.SaveChangesAsync();
                await log.Information($"Allergen.Update success: id={entity.Id}");
                return Ok($"Successful update of {entity.Name} in {entity.Id}");
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "Allergen.Update, database connection error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"Allergen.Update failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id, [FromServices] IAppLogger log)
        {
            try
            {
                var entity = await _context.Allergen.FindAsync(id);
                if (entity == null) return NotFound();

                // If there is a FK from FoodAllergen, you might need to handle cascade or restrict here
                _context.Allergen.Remove(entity);
                await _context.SaveChangesAsync();
                await log.Information($"Allergen.Delete success: id={id}");
                return Ok($"Successfully deletion of {entity.Name}");
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "Allergen.Delete, database connection error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"Allergen.Delete failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
