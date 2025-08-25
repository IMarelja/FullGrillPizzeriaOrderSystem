using GrillPizzeriaOrderWebApp.Services.IAPIs;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.APIs
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        public const string BasePath = "/api/auth";
        public AuthService(HttpClient http) => _http = http;

        public Task<string?> LoginAsync(LoginViewModel vm)
        {
            throw new NotImplementedException();
        }
        public Task RegisterAsync(RegisterViewModel vm)
        {
            throw new NotImplementedException();
        }

        public Task<UserViewNormalModel?> MeAsync()
        {
            throw new NotImplementedException();
        }

        
    }
}
