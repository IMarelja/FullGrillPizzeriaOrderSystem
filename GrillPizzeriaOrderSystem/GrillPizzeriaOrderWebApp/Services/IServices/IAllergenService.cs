using GrillPizzeriaOrderWebApp.Models;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.IServices
{
    public interface IAllergenService
    {
        Task<IEnumerable<AllergenViewModel>> GetAll();
        Task<AllergenViewModel?> GetById(int id);
        Task<ApiOperationResult<int>> CreateAsync(AllergenCreateViewModel food);
        Task<ApiOperationResult> UpdateAsync(AllergenEditViewModel food);
        Task<ApiOperationResult> DeleteAsync(int id);
    }
}
