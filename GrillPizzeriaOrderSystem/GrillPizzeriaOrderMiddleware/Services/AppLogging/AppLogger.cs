using GrillPizzeriaOrderMiddleware.DatabaseContexts;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace GrillPizzeriaOrderMiddleware.Services.AppLogging
{
    public class AppLogger : IAppLogger
    {
        private readonly GrillPizzaDatabaseContext _db;
        private readonly ILogger<AppLogger> _logger;

        public AppLogger(GrillPizzaDatabaseContext db, ILogger<AppLogger> logger)
        {
            _db = db;
            _logger = logger;
        }

        private async Task WriteAsync(string level, string message)
        {
            // Framework logger honors appsettings.json
            switch (level)
            {
                case "Warning": _logger.LogWarning("{Message}", message); break;
                case "Error": _logger.LogError("{Message}", message); break;
                default: _logger.LogInformation("{Message}", message); break;
            }

            // Persist to DB
            _db.Log.Add(new Log { Level = level, Message = message }); // Timestamp set by model
            await _db.SaveChangesAsync();
        }

        public Task Information(string message) => WriteAsync("Information", message);
        public Task Warning(string message) => WriteAsync("Warning", message);
        public Task Error(string message) => WriteAsync("Error", message);
    }
}
