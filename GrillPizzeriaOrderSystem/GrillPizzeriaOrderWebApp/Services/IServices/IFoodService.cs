using GrillPizzeriaOrderWebApp.Models;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.IServices
{
    public interface IFoodService
    {
        Task<IReadOnlyList<FoodViewModel>> GetAll();
        Task<FoodViewModel?> GetById(int id);
        Task<ApiOperationResult<int>> CreateAsync(FoodCreateViewModel food);
        Task<ApiOperationResult> UpdateAsync(FoodEditViewModel food);
        Task<ApiOperationResult> DeleteAsync(FoodDeleteViewModel food);

        Task<FoodSearchViewModel> SearchFilterAsync(string? search, int? categoryId);
        Task<FoodSearchViewModel> SearchPageFilter(string? search, int? categoryId, int page, int pageSize);

    }
}
