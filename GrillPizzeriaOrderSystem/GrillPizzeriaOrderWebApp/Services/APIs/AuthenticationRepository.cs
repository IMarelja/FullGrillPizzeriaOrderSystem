using System.Net.Http.Headers;
using System.Text.Json;
using GrillPizzeriaOrderWebApp.Models;
using GrillPizzeriaOrderWebApp.Services.IServices;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.APIs
{
    public class AuthenticationRepository : IAuthenticationGrillService
    {
        private readonly HttpClient _client;
        private const string EndPoint = "Authentication";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AuthenticationRepository(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthenticationResult> LoginAsync(LoginViewModel loginUser)
        {
            try
            {
                var authResp = await _client.PostAsJsonAsync($"{EndPoint}/login", new
                {
                    loginUser.Username,
                    loginUser.Password
                });

                var authRaw = await authResp.Content.ReadAsStringAsync();

                if (!authResp.IsSuccessStatusCode)
                {
                    return new AuthenticationResult
                    {
                        Success = false,
                        Message = $"Login failed {(int)authResp.StatusCode}: {authRaw}",
                        StatusCode = (int)authResp.StatusCode
                    };
                }

                var authData = JsonSerializer.Deserialize<Dictionary<string, string>>(authRaw,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (authData == null || !authData.TryGetValue("token", out var jwt))
                {
                    return new AuthenticationResult
                    {
                        Success = false,
                        Message = "Invalid login response."
                    };
                }

                // Get user profile
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
                var profileResp = await _client.GetAsync("User/");

                if (!profileResp.IsSuccessStatusCode)
                {
                    return new AuthenticationResult
                    {
                        Success = false,
                        Message = $"Failed to load profile: {profileResp.StatusCode}",
                        StatusCode = (int)profileResp.StatusCode
                    };
                }

                var profileRaw = await profileResp.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(profileRaw);
                var root = doc.RootElement;

                var userProfile = new UserViewModel
                {
                    id = root.GetProperty("id").GetInt32(),
                    username = root.GetProperty("username").GetString() ?? string.Empty,
                    roleName = root.GetProperty("roleName").GetString() ?? string.Empty
                };

                return new AuthenticationResult
                {
                    Success = true,
                    Token = jwt,
                    UserProfile = userProfile
                };
            }
            catch (Exception ex)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Message = $"An error occurred during login: {ex.Message}"
                };
            }
        }

        public async Task<AuthenticationResult> RegisterAsync(RegisterViewModel registerUser)
        {
            try
            {
                var resp = await _client.PostAsJsonAsync($"{EndPoint}/register", new
                {
                    registerUser.Username,
                    registerUser.FirstName,
                    registerUser.LastName,
                    registerUser.Phone,
                    registerUser.Email,
                    registerUser.Password,
                    registerUser.ConfirmPassword
                });

                var raw = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    return new AuthenticationResult
                    {
                        Success = false,
                        Message = $"Registration failed {(int)resp.StatusCode}: {raw}",
                        StatusCode = (int)resp.StatusCode
                    };
                }

                return new AuthenticationResult
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Message = $"An error occurred during registration: {ex.Message}"
                };
            }
        }

        public async Task<ApiOperationResult> ChangePasswordAsync(ChangePasswordViewModel request)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.PostAsJsonAsync($"{EndPoint}/changepassword", request);

            if (!response.IsSuccessStatusCode)
            {
                string raw = await response.Content.ReadAsStringAsync();
                return ApiOperationResult.Fail($"Password change failed ({(int)response.StatusCode}): {raw}");
            }

            return ApiOperationResult.Ok("Password change SUCCESSFUL");
        }
    }
}
