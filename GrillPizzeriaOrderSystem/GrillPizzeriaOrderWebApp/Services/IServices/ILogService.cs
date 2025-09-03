using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.IServices
{
    public interface ILogService
    {
        Task<IReadOnlyList<LogViewModel>> GetLogsAsync(int numberOfLogs);
        Task<int> GetLogCountAsync();
    }
}
