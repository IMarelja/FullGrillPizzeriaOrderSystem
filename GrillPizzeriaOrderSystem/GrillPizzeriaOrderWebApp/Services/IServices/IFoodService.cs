using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.IServices
{
    public interface IFoodService
    {
        Task<IEnumerable<FoodViewModel>> GetAll();
        Task<FoodViewModel?> GetById(int id);
        Task UpdateAsync(FoodEditViewModel food);
        Task CreateAsync(FoodCreateViewModel food);
        Task DeleteAsync(int id);
        Task<IEnumerable<FoodViewModel>> GetByFilterSearch(string? search, int? genreId, int page, int pageSize);

    }
}
