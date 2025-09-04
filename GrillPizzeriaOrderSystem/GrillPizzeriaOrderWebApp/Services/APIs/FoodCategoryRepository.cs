using System.Net;
using System.Text.Json;
using GrillPizzeriaOrderWebApp.Models;
using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.APIs
{
    public class FoodCategoryRepository : IFoodCategoryService
    {
        private readonly HttpClient _client;
        public const string EndPoint = "FoodCategory";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public FoodCategoryRepository(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<CategoryFoodViewModel>> GetAll()
        {
            var response = await _client.GetAsync(EndPoint);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return Enumerable.Empty<CategoryFoodViewModel>();

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            var items = await JsonSerializer.DeserializeAsync<List<CategoryFoodViewModel>>(stream, _jsonOpts);
            return items ?? new List<CategoryFoodViewModel>();
        }

        public async Task<CategoryFoodViewModel?> GetById(int id)
        {
            var response = await _client.GetAsync($"{EndPoint}/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<CategoryFoodViewModel>(stream, _jsonOpts);
        }

        public async Task<ApiOperationResult<int>> CreateAsync(CategoryFoodCreateViewModel food)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.PostAsJsonAsync(EndPoint, food);

            if (!response.IsSuccessStatusCode)
            {
                string raw = await response.Content.ReadAsStringAsync();
                return ApiOperationResult<int>.Fail($"Create failed ({(int)response.StatusCode}): {raw}");
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            var vm = await JsonSerializer.DeserializeAsync<CategoryFoodViewModel>(stream, _jsonOpts);

            if (vm is null)
                return ApiOperationResult<int>.Fail("Empty response from server.");

            return ApiOperationResult<int>.Ok(vm.id, "Allergen created successfully.");
        }

        public async Task<ApiOperationResult> UpdateAsync(CategoryFoodEditViewModel foodCategory)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.PutAsJsonAsync($"{EndPoint}/{foodCategory.id}", foodCategory);

            if (!response.IsSuccessStatusCode)
            {
                string raw = await response.Content.ReadAsStringAsync();
                return ApiOperationResult.Fail($"Update failed ({(int)response.StatusCode}): {raw}");
            }
            
            return ApiOperationResult.Ok("Food category updated successfully.");
        }

        public async Task<ApiOperationResult> DeleteAsync(CategoryFoodDeleteViewModel foodCategory)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.DeleteAsync($"{EndPoint}/{foodCategory.id}");

            if (!response.IsSuccessStatusCode)
            {
                string raw = await response.Content.ReadAsStringAsync();
                return ApiOperationResult.Fail($"Delete failed ({(int)response.StatusCode}): {raw}");
            }

            return ApiOperationResult.Ok("Food category deleted successfully.");
        }
    }
}
