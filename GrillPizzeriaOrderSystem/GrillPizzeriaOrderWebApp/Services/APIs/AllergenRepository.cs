using System.Text.Json;
using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.APIs
{
    public class AllergenRepository : IAllergenService
    {
        private readonly HttpClient _client;
        public const string EndPoint = "Allergen";
        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };
        public AllergenRepository(HttpClient httpClient) => _client = httpClient;

        public Task CreateAsync(AllergenCreateViewModel food)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AllergenViewModel>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<AllergenViewModel?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(AllergenEditViewModel food)
        {
            throw new NotImplementedException();
        }
    }
}
