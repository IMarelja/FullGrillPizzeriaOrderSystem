using GrillPizzeriaOrderWebApp.Models;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.IServices
{
    public interface IFoodCategoryService
    {
        Task<IEnumerable<CategoryFoodViewModel>> GetAll();
        Task<CategoryFoodViewModel?> GetById(int id);
        Task<ApiOperationResult<int>> CreateAsync(CategoryFoodCreateViewModel categoryFood);
        Task<ApiOperationResult> UpdateAsync(CategoryFoodEditViewModel categoryFood);
        Task<ApiOperationResult> DeleteAsync(CategoryFoodDeleteViewModel categoryFood);

    }
}
