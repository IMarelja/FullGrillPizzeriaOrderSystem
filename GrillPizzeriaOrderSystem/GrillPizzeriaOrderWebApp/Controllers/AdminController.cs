using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrillPizzeriaOrderWebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogService _logService;

        public AdminController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Index() => View();

        // Returns the logs table (partial)
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetLogs(int number = 10)
        {
            var logs = await _logService.GetLogsAsync(number);
            return PartialView("_LogsTable", logs);
        }

        // Returns the log count (partial)
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetLogCount()
        {
            var count = await _logService.GetLogCountAsync();
            return PartialView("_LogCount", count);
        }
    }
}
