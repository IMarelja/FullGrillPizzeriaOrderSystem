using System.Net.Http;
using System.Text.Json;
using System.Xml.Linq;
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
