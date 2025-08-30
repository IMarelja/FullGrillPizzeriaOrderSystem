using GrillPizzeriaOrderWebApp.Models;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.IServices
{
    public interface IAuthenticationGrillService
    {
        Task<AuthenticationResult> LoginAsync(LoginViewModel loginUser);
        Task<AuthenticationResult> RegisterAsync(RegisterViewModel registerUser);
        Task<AuthenticationResult> ChangePasswordAsync(ChangePasswordViewModel request);
    }
}
