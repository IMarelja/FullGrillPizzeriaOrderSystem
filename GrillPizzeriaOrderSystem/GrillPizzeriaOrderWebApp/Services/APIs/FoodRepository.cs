using System.Net;
using System.Text.Json;
using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.WebUtilities;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.APIs
{
    public class FoodRepository : IFoodService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public const string EndPoint = "Food";
        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public FoodRepository(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IReadOnlyList<FoodViewModel>> GetAll()
        {
            var response = await _client.GetAsync(EndPoint);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            var items = await JsonSerializer.DeserializeAsync<List<FoodViewModel>>(stream, _jsonOpts);
            return items ?? new List<FoodViewModel>();
        }

        public async Task<FoodViewModel?> GetById(int id)
        {
            var response = await _client.GetAsync($"{EndPoint}/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<FoodViewModel>(stream, _jsonOpts);
        }

        public async Task CreateAsync(FoodCreateViewModel food)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.PostAsJsonAsync(EndPoint, food, _jsonOpts);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(FoodEditViewModel food)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.PutAsJsonAsync($"{EndPoint}/{food.id}", food, _jsonOpts);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.DeleteAsync($"{EndPoint}/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<FoodSearchViewModel> SearchFilterAsync(string? search, int? categoryId)
        {
            var qs = new Dictionary<string, string?>();
            if (!string.IsNullOrWhiteSpace(search)) qs["q"] = search;
            if (categoryId.HasValue) qs["categoryId"] = categoryId.Value.ToString();

            var url = QueryHelpers.AddQueryString($"{EndPoint}/search", qs!);

            using var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<FoodSearchViewModel>(stream, _jsonOpts);
            return result;
        }

        public async Task<FoodSearchViewModel> SearchPageFilter(string? search, int? categoryId, int page, int pageSize)
        {
            var qs = new Dictionary<string, string?>
            {
                ["page"] = page.ToString(),
                ["pageSize"] = pageSize.ToString()
            };
            if (!string.IsNullOrWhiteSpace(search)) qs["q"] = search.Trim();
            if (categoryId.HasValue && categoryId.Value > 0) qs["categoryId"] = categoryId.Value.ToString();

            var url = QueryHelpers.AddQueryString($"{EndPoint}/search", qs!);

            using var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<FoodSearchViewModel>(stream, _jsonOpts);
            return result;
        }
    }

}
