using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.IServices
{
    public interface IAllergenService
    {
        Task<IEnumerable<AllergenViewModel>> GetAll();
        Task<AllergenViewModel?> GetById(int id);
        Task UpdateAsync(AllergenEditViewModel food);
        Task CreateAsync(AllergenCreateViewModel food);
        Task DeleteAsync(int id);
    }
}
