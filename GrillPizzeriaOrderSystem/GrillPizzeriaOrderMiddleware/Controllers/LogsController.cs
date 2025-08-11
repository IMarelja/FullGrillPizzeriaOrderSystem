﻿using AutoMapper;
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
    [Authorize]
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
            var logs = await _context.Logs
                .OrderByDescending(l => l.Timestamp)
                .Take(n)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<LogReadDto>>(logs);
            return Ok(dtos);
        }

        /// <summary>
        /// Returns the total number of log entries.
        /// </summary>
        [HttpGet("count")]
        public async Task<ActionResult<int>> Count()
        {
            var total = await _context.Logs.CountAsync();
            return Ok(total);
        }
    }
}
