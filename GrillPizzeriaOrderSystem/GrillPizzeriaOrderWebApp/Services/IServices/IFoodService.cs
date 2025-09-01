using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.IServices
{
    public interface IFoodService
    {
        Task<IReadOnlyList<FoodViewModel>> GetAll();
        Task<FoodViewModel?> GetById(int id);
        Task CreateAsync(FoodCreateViewModel food);
        Task UpdateAsync(FoodEditViewModel food);
        Task DeleteAsync(int id);

        Task<FoodSearchViewModel> SearchFilterAsync(string? search, int? categoryId);
        Task<FoodSearchViewModel> SearchPageFilter(string? search, int? categoryId, int page, int pageSize);

    }
}
