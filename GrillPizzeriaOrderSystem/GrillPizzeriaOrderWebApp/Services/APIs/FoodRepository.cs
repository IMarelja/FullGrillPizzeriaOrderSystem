using GrillPizzeriaOrderWebApp.Services.IServices;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.APIs
{
    public class FoodRepository : IFoodService
    {
        private readonly HttpClient _client;
        public const string EndPoint = "Food";

        public FoodRepository(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<FoodViewModel>> GetAll()
        {
            var response = await _client.GetAsync(EndPoint);

            return null;
        }

        public Task<FoodViewModel?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(FoodEditViewModel food)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(FoodCreateViewModel food)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FoodViewModel>> GetByFilterSearch(string? search, int? genreId, int page, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
