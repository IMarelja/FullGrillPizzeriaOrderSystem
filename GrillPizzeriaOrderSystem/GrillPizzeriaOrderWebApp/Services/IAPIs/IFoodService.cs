using ViewModels;

namespace GrillPizzeriaOrderWebApp.Services.IAPIs
{
    public interface IFoodService
    {
        Task<IEnumerable<FoodViewModel>> Find();
    }
}
