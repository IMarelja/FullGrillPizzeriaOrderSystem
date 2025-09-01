using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrillPizzeriaOrderWebApp.Controllers
{
    public class FoodCategoryController : Controller
    {
        private readonly IFoodCategoryService _foodCategoryService;

        public FoodCategoryController(IFoodCategoryService foodCategoryService)
        {
            _foodCategoryService = foodCategoryService;
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
