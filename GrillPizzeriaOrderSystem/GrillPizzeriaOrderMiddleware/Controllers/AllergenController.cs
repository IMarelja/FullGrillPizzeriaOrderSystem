using AutoMapper;
using DTO.Allergen;
using GrillPizzeriaOrderMiddleware.DatabaseContexts;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<IEnumerable<AllergenReadDto>>> GetAll()
        {
            var items = await _context.Allergen
                .OrderBy(a => a.Name)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<AllergenReadDto>>(items));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AllergenReadDto>> GetById(int id)
        {
            var entity = await _context.Allergen.FindAsync(id);
            if (entity == null) 
                return NotFound();
            return Ok(_mapper.Map<AllergenReadDto>(entity));
        }

        [HttpPost]
        public async Task<ActionResult<AllergenReadDto>> Create([FromBody] AllergenCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existsAllergen = await _context.Allergen.AnyAsync(a => a.Name == createDto.Name);
            if (existsAllergen) 
                return BadRequest("Allergen with the same name already exists.");

            var entity = _mapper.Map<Allergen>(createDto);
            _context.Allergen.Add(entity);
            await _context.SaveChangesAsync();

            var read = _mapper.Map<AllergenReadDto>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, read);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AllergenCreateDto updateDto)
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
                    return BadRequest("Allergen with the same name already exists.");
            }

            _mapper.Map(updateDto, entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Allergen.FindAsync(id);
            if (entity == null) return NotFound();

            // If there is a FK from FoodAllergen, you might need to handle cascade or restrict here
            _context.Allergen.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
