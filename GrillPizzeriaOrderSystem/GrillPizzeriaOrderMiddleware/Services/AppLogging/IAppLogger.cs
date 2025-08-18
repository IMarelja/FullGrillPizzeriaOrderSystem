using Microsoft.AspNetCore.Mvc;

namespace GrillPizzeriaOrderMiddleware.Services.AppLogging
{
    public interface IAppLogger
    {
        Task Information(string message);
        Task Warning(string message);
        Task Error(string message);
    }
}
