using GrillPizzeriaOrderWebApp.Models;
using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Controllers
{
    public class FoodCategoryController : Controller
    {
        private readonly IFoodCategoryService _foodCategoryService;

        public FoodCategoryController(IFoodCategoryService foodCategoryService)
        {
            _foodCategoryService = foodCategoryService;
        }

        [Authorize(Policy = "AdminOnly")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(CategoryFoodCreateViewModel category)
        {
            if (!ModelState.IsValid)
            {
                var message = ApiOperationResult.Fail("Creation validation error.");
                return View("~/Views/FoodCategory/Index.cshtml", message);
            }

            var response = await _foodCategoryService.CreateAsync(category);

            return View("~/Views/FoodCategory/Index.cshtml", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(CategoryFoodEditViewModel category)
        {
            if (!ModelState.IsValid)
            {
                var message = ApiOperationResult.Fail("Editing validation error.");
                return View("~/Views/FoodCategory/Index.cshtml", message);
            }

            var response = await _foodCategoryService.UpdateAsync(category);

            return View("~/Views/FoodCategory/Index.cshtml", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(CategoryFoodDeleteViewModel category)
        {
            if (!ModelState.IsValid)
            {
                var message = ApiOperationResult.Fail("Deletion validation error.");
                return View("~/Views/FoodCategory/Index.cshtml", message);
            }

            var response = await _foodCategoryService.DeleteAsync(category);

            return View("~/Views/FoodCategory/Index.cshtml", response);
        }

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var categories = await _foodCategoryService.GetAll(); // Task<IEnumerable<CategoryFoodViewModel>>
                return Ok(new
                {
                    success = true,
                    data = categories,
                    message = "Food categories loaded successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError , new 
                {
                    success = false,
                    message = "Error loading food categories: " + ex.Message
                });
            }
        }
    }
}
