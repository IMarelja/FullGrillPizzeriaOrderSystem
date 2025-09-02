using AutoMapper;
using DTO.Log;
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
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<LogReadDto>>> GetLast(int n, [FromServices] IAppLogger log)
        {
            try
            {
                var logs = await _context.Log
                        .OrderByDescending(l => l.Timestamp)
                        .Take(n)
                        .ToListAsync();

                var dtos = _mapper.Map<IEnumerable<LogReadDto>>(logs);
                return Ok(dtos);
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "Log.GetLast, database connection error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"Log.GetLast failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("count")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<int>> Count([FromServices] IAppLogger log)
        {
            try
            {
                var total = await _context.Log.CountAsync();
                return Ok(total);
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 2)
            {
                return StatusCode(503, "Log.Count, database connection error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await log.Error($"Log.Count failed: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
