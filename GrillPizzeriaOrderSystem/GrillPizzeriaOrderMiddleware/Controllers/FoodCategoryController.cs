using AutoMapper;
using DTO.FoodCategory;
using GrillPizzeriaOrderMiddleware.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace GrillPizzeriaOrderMiddleware.Controllers
{
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
        [Authorize]
        public async Task<ActionResult<FoodCategoryReadDto>> Create([FromBody] FoodCategoryCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = _mapper.Map<FoodCategory>(createDto);
            _context.FoodCategory.Add(entity);
            await _context.SaveChangesAsync();

            var readDto = _mapper.Map<FoodCategoryReadDto>(entity);
            return CreatedAtAction(nameof(Get), new { id = readDto.Id }, readDto);
        }

        // Read all
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodCategoryReadDto>>> GetAll()
        {
            var entities = await _context.FoodCategory.ToListAsync();
            var dtos = _mapper.Map<IEnumerable<FoodCategoryReadDto>>(entities);
            return Ok(dtos);
        }

        // Read one
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodCategoryReadDto>> Get(int id)
        {
            var entity = await _context.FoodCategory.FindAsync(id);
            if (entity == null)
                return NotFound();

            var dto = _mapper.Map<FoodCategoryReadDto>(entity);
            return Ok(dto);
        }

        // Update
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] FoodCategoryCreateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = await _context.FoodCategory.FindAsync(id);
            if (entity == null)
                return NotFound();

            if (!string.Equals(entity.Name, updateDto.Name, StringComparison.OrdinalIgnoreCase) &&
                await _context.FoodCategory.AnyAsync(c => c.Name == updateDto.Name))
            {
                return BadRequest("Category with the same name already exists.");
            }

            _mapper.Map(updateDto, entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// Deletes
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.FoodCategory.FindAsync(id);
            if (entity == null)
                return NotFound();

            _context.FoodCategory.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
