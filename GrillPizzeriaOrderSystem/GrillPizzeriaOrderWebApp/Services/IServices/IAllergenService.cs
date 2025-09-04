using GrillPizzeriaOrderWebApp.Models;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.IServices
{
    public interface IAllergenService
    {
        Task<IEnumerable<AllergenViewModel>> GetAll();
        Task<AllergenViewModel?> GetById(int id);
        Task<ApiOperationResult<int>> CreateAsync(AllergenCreateViewModel allergen);
        Task<ApiOperationResult> UpdateAsync(AllergenEditViewModel allergen);
        Task<ApiOperationResult> DeleteAsync(AllergenDeleteViewModel allergen);
    }
}
