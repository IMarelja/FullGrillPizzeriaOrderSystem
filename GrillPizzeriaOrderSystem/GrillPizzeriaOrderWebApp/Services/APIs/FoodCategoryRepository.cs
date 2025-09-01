using System.Net;
using System.Text.Json;
using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.APIs
{
    public class FoodCategoryRepository : IFoodCategoryService
    {
        private readonly HttpClient _client;
        public const string EndPoint = "FoodCategory";
        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public FoodCategoryRepository(HttpClient client) => _client = client;

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

        public async Task CreateAsync(CategoryFoodCreateViewModel food)
        {
            var response = await _client.PostAsJsonAsync(EndPoint, food);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(CategoryFoodEditViewModel food)
        {
            var response = await _client.PutAsJsonAsync($"{EndPoint}/{food.id}", food);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _client.DeleteAsync($"{EndPoint}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
