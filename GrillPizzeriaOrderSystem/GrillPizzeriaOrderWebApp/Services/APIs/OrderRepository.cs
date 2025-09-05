using System.Net;
using System.Text.Json;
using GrillPizzeriaOrderWebApp.Models;
using GrillPizzeriaOrderWebApp.Services.IServices;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.APIs
{
    public class OrderRepository : IOrderService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public const string EndPoint = "Order";
        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };
        public OrderRepository(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiOperationResult<OrderViewModel>> CreateAsync(OrderCreateViewModel model)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.PostAsJsonAsync(EndPoint, model);

            if (!response.IsSuccessStatusCode)
            {
                string raw = await response.Content.ReadAsStringAsync();
                return ApiOperationResult<OrderViewModel>.Fail($"Create failed ({(int)response.StatusCode}): {raw}");
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            var vm = await JsonSerializer.DeserializeAsync<OrderViewModel>(stream, _jsonOpts);

            if (vm is null)
                return ApiOperationResult<OrderViewModel>.Fail("Empty response from server.");

            return ApiOperationResult<OrderViewModel>.Ok(vm, "Order shipped successfully.");
        }

        public async Task<IEnumerable<OrderViewModel>> GetAllAsync()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.GetAsync($"{EndPoint}/all");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Enumerable.Empty<OrderViewModel>();

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            var items = await JsonSerializer.DeserializeAsync<List<OrderViewModel>>(stream, _jsonOpts);
            return items ?? new List<OrderViewModel>();
        }

        public async Task<OrderViewModel?> GetByIdAsync(int id)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.GetAsync($"{EndPoint}/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<OrderViewModel>(stream, _jsonOpts);
        }

        public async Task<ApiOperationResult> DeleteAsync(OrderDeleteViewModel model)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.DeleteAsync($"{EndPoint}/{model.id}");

            if (!response.IsSuccessStatusCode)
            {
                string raw = await response.Content.ReadAsStringAsync();
                return ApiOperationResult.Fail($"Delete failed ({(int)response.StatusCode}): {raw}");
            }

            return ApiOperationResult.Ok("Order canceled successfully.");
        }

        public async Task<IEnumerable<OrderViewModel>> GetUserOrdersAsync()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.GetAsync($"{EndPoint}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Enumerable.Empty<OrderViewModel>();

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            var items = await JsonSerializer.DeserializeAsync<List<OrderViewModel>>(stream, _jsonOpts);
            return items ?? new List<OrderViewModel>();
        }
    }
}
