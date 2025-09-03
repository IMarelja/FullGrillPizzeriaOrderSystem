using System.Net;
using System.Net.Http;
using System.Text.Json;
using GrillPizzeriaOrderWebApp.Models;
using GrillPizzeriaOrderWebApp.Services.IServices;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.APIs
{
    public class UserRepository : IUserService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public const string EndPoint = "User";
        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };
        public UserRepository(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserViewModel?> GetMeAsync()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.GetAsync(EndPoint);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UserViewModel>(json, _jsonOpts);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return null;
        }

        public async Task<IEnumerable<UserViewModel>> GetAllAsync()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.GetAsync($"{EndPoint}/all");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return Enumerable.Empty<UserViewModel>();

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            var items = await JsonSerializer.DeserializeAsync<List<UserViewModel>>(stream, _jsonOpts);
            return items ?? new List<UserViewModel>();
        }

        public async Task<ApiOperationResult<UserViewModel>> UpdateMe(UserUpdateViewModel user)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.PutAsJsonAsync($"{EndPoint}/me", user);

            if (response.IsSuccessStatusCode)
            {
                using var stream = await response.Content.ReadAsStreamAsync();
                var vm = await JsonSerializer.DeserializeAsync<AllergenViewModel>(stream, _jsonOpts);

                if (vm is null)
                    return ApiOperationResult<UserViewModel>.Fail("Empty response from server.");

            }

            string raw = await response.Content.ReadAsStringAsync();
            return ApiOperationResult<UserViewModel>.Fail($"Update failed ({(int)response.StatusCode}): {raw}");
        }
    }
}
