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

        public AuthenticationRepository(HttpClient client)
        {
            _client = client;
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
                        ErrorMessage = $"Login failed {(int)authResp.StatusCode}: {authRaw}",
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
                        ErrorMessage = "Invalid login response."
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
                        ErrorMessage = $"Failed to load profile: {profileResp.StatusCode}",
                        StatusCode = (int)profileResp.StatusCode
                    };
                }

                var profileRaw = await profileResp.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(profileRaw);
                var root = doc.RootElement;

                var userProfile = new UserProfile
                {
                    Id = root.GetProperty("id").GetInt32(),
                    Username = root.GetProperty("username").GetString() ?? string.Empty,
                    RoleName = root.GetProperty("roleName").GetString() ?? string.Empty
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
                    ErrorMessage = $"An error occurred during login: {ex.Message}"
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
                        ErrorMessage = $"Registration failed {(int)resp.StatusCode}: {raw}",
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
                    ErrorMessage = $"An error occurred during registration: {ex.Message}"
                };
            }
        }

        public async Task<AuthenticationResult> ChangePasswordAsync(ChangePasswordViewModel request)
        {
            try
            {
                var resp = await _client.PostAsJsonAsync($"{EndPoint}/changepassword", new
                {
                    request.CurrentPassword,
                    request.NewPassword
                });

                var raw = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    return new AuthenticationResult
                    {
                        Success = false,
                        ErrorMessage = $"Password change failed {(int)resp.StatusCode}: {raw}",
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
                    ErrorMessage = $"An error occurred during password change: {ex.Message}"
                };
            }
        }
    }
}
