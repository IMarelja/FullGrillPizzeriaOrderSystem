using GrillPizzeriaOrderWebApp.Models;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.IServices
{
    public interface IUserService
    {
        Task<UserViewModel?> GetMeAsync();
        Task<IEnumerable<UserViewModel>> GetAllAsync();
        Task<ApiOperationResult<UserViewModel>> UpdateMe(UserUpdateViewModel user);
    }
}
