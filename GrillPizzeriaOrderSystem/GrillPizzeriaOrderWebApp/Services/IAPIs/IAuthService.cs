using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.IAPIs
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginViewModel vm); // returns JWT
        Task RegisterAsync(RegisterViewModel vm);
        Task<UserViewNormalModel?> MeAsync(); // optional
    }
}
