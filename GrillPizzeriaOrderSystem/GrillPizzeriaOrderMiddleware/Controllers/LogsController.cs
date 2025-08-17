using AutoMapper;
using DTO.Log;
using GrillPizzeriaOrderMiddleware.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace GrillPizzeriaOrderMiddleware.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class LogsController : ControllerBase
    {
        private readonly GrillPizzaDatabaseContext _context;
        private readonly IMapper _mapper;

        public LogsController(GrillPizzaDatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("get/{n}")]
        public async Task<ActionResult<IEnumerable<LogReadDto>>> GetLast(int n)
        {
            var logs = await _context.Log
                .OrderByDescending(l => l.Timestamp)
                .Take(n)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<LogReadDto>>(logs);
            return Ok(dtos);
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> Count()
        {
            var total = await _context.Log.CountAsync();
            return Ok(total);
        }
    }
}
