using GrillPizzeriaOrderWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.IServices
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderViewModel>> GetAllAsync();
        Task<OrderViewModel?> GetByIdAsync(int id);
        Task<ApiOperationResult<OrderViewModel>> CreateAsync(OrderCreateViewModel model);
        Task<ApiOperationResult> DeleteAsync(OrderDeleteViewModel model);
        Task<IEnumerable<OrderViewModel>> GetUserOrdersAsync();
    }
}
