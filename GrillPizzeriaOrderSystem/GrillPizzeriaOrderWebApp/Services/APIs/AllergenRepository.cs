using System.Net;
using System.Text.Json;
using GrillPizzeriaOrderWebApp.Models;
using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.APIs
{
    public class AllergenRepository : IAllergenService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public const string EndPoint = "Allergen";
        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };
        public AllergenRepository(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
        }

        // doesnt require authentication, anonymous access
        public async Task<IEnumerable<AllergenViewModel>> GetAll()
        {
            var response = await _client.GetAsync(EndPoint);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return Enumerable.Empty<AllergenViewModel>();

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            var items = await JsonSerializer.DeserializeAsync<List<AllergenViewModel>>(stream, _jsonOpts);
            return items ?? new List<AllergenViewModel>();
        }

        public async Task<AllergenViewModel?> GetById(int id)
        {
            var response = await _client.GetAsync($"{EndPoint}/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<AllergenViewModel>(stream, _jsonOpts);
        }

        // requires authentication
        public async Task<ApiOperationResult<int>> CreateAsync(AllergenCreateViewModel allergen)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.PostAsJsonAsync(EndPoint, allergen);

            if (response.IsSuccessStatusCode)
            {
                using var stream = await response.Content.ReadAsStreamAsync();
                var vm = await JsonSerializer.DeserializeAsync<AllergenViewModel>(stream, _jsonOpts);

                if (vm is null)
                    return ApiOperationResult<int>.Fail("Empty response from server.");

                return ApiOperationResult<int>.Ok(vm.id, "Allergen created successfully.");
            }

            // handle errors
            string raw = await response.Content.ReadAsStringAsync();
            return ApiOperationResult<int>.Fail($"Create failed ({(int)response.StatusCode}): {raw}");
        }

        public async Task<ApiOperationResult> UpdateAsync(AllergenEditViewModel allergen)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.PutAsJsonAsync($"{EndPoint}/{allergen.id}", allergen);

            if (response.IsSuccessStatusCode)
                return ApiOperationResult.Ok("Allergen updated successfully.");

            string raw = await response.Content.ReadAsStringAsync();
            return ApiOperationResult.Fail($"Update failed ({(int)response.StatusCode}): {raw}");
        }

        public async Task<ApiOperationResult> DeleteAsync(int id)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.DeleteAsync($"{EndPoint}/{id}");

            if (response.IsSuccessStatusCode)
                return ApiOperationResult.Ok("Allergen deleted successfully.");

            string raw = await response.Content.ReadAsStringAsync();
            return ApiOperationResult.Fail($"Delete failed ({(int)response.StatusCode}): {raw}");
        }
    }
}
