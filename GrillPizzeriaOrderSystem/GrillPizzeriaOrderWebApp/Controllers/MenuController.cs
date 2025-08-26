using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Controllers
{
    public class MenuController : Controller
    {
        private readonly IHttpClientFactory _http;
        public MenuController(IHttpClientFactory http) => _http = http;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var client = _http.CreateClient("DataAPI");

            List<FoodViewModel> foodList;
            List<CategoryFoodViewModel> categoryFoodList;

            try
            {
                categoryFoodList = new List<CategoryFoodViewModel>();
            }
            catch
            {
                categoryFoodList = new List<CategoryFoodViewModel>();
            }

            ViewBag.CategoryFoodList = categoryFoodList;

            try
            {
                foodList = new List<FoodViewModel>();
            }
            catch
            {
                foodList = new List<FoodViewModel>();
            }

            ViewBag.FoodList = foodList;

            return View();
        }
    }
}
