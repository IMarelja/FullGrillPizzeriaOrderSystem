using GrillPizzeriaOrderWebApp.Services.IAPIs;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.APIs
{
    public class FoodService : IFoodService
    {
        private readonly HttpClient _client;
        public const string BasePath = "/api/find";

        public FoodService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<FoodViewModel>> Find()
        {
            var response = await _client.GetAsync(BasePath);

            return null;
        }
    }
}
