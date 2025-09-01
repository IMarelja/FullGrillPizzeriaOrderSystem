using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.IServices
{
    public interface IFoodCategoryService
    {
        Task<IEnumerable<CategoryFoodViewModel>> GetAll();
        Task<CategoryFoodViewModel?> GetById(int id);
        Task UpdateAsync(CategoryFoodEditViewModel food);
        Task CreateAsync(CategoryFoodCreateViewModel food);
        Task DeleteAsync(int id);

    }
}
