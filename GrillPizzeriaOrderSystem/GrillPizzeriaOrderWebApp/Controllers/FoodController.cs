using System.Net.Http;
using System.Text.Json;
using System.Xml.Linq;
using GrillPizzeriaOrderWebApp.Models;
using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Controllers
{
    public class FoodController : Controller
    {
        private readonly IFoodService _foodService;
       
        public FoodController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        public IActionResult Index()
            => View("CreateFood.cshtml");

        public async Task<IActionResult> Create(FoodCreateViewModel food)
        {
            if (!ModelState.IsValid)
            {
                var message = ApiOperationResult.Fail("Creation validation error.");
                return View("~/Views/Food/CreateFood.cshtml", message);
            }

            var responce = await _foodService.CreateAsync(food);

            return View("~/Views/Food/CreateFood.cshtml", new ApiOperationResult<FoodCreateViewModel> { Succeeded = true });
        }

        public async Task<IActionResult > Delete(int foodId)
        {


            FoodDeleteViewModel foodDeleteViewModel = new FoodDeleteViewModel() { id = foodId };

            var responce = await _foodService.DeleteAsync(foodDeleteViewModel);

            if (!responce.Succeeded)
                return RedirectToAction("Index");

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string? search, int? foodCategoryId)
        {
            var foods = await _foodService.SearchFilterAsync(search, foodCategoryId);
            return PartialView("_FoodListGrid", foods);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PageSearch(string? search, int? foodCategoryId, int page = 1, int pageSize = 10)
        {
            var foods = await _foodService.SearchPageFilter(search, foodCategoryId, page, pageSize);
            return PartialView("_FoodListGrid", foods);
        }
    }
}
