using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Http;
using NuGet.Common;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.APIs
{
    public class LogRepository : ILogService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public const string EndPoint = "Logs";
        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };
        public LogRepository(HttpClient client, IHttpContextAccessor httpContextAccessor) 
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
        } 

        public async Task<IReadOnlyList<LogViewModel>> GetLogsAsync(int numberOfLogs)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.GetAsync($"{EndPoint}/get/{numberOfLogs}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return Array.Empty<LogViewModel>();

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            var items = await JsonSerializer.DeserializeAsync<IReadOnlyList<LogViewModel>>(stream, _jsonOpts);
            return items ?? Array.Empty<LogViewModel>();
        }

        public async Task<int> GetLogCountAsync()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.GetAsync($"{EndPoint}/count");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return 0;

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            if (int.TryParse(content.Trim(), out int count))
                return count;

            return 0;
        }
    }
}
