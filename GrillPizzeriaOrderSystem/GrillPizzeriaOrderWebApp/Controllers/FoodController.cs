using System.Net.Http;
using System.Text.Json;
using System.Xml.Linq;
using GrillPizzeriaOrderWebApp.Models;
using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
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

        [HttpPost]
        
        public IActionResult Index()
            => View("~/Views/Food/CreateFood.cshtml");

        [Authorize(Policy = "AdminOnly")]
        public IActionResult CreateFood()
        {
            return View();
        }

        [Authorize(Policy = "AdminOnly")]
        public IActionResult EditFood()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(FoodCreateViewModel food)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                        .Where(ms => ms.Value?.Errors.Count > 0)
                        .Select(ms => new
                        {
                            Field = ms.Key,
                            Errors = ms.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        });

                return BadRequest(new
                {
                    success = false,
                    message = "Invalid Food request",
                    errors
                });
            }

            var responce = await _foodService.CreateAsync(food);

            return Ok(new
            {
                success = responce.Succeeded,
                data = responce,
                message = responce.Message + " " + string.Join(", ", responce.Errors),
            });
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> EditFoodView(int id)
        {
            var food = await _foodService.GetById(id);
            return View("EditFood", food);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(FoodEditViewModel food)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                        .Where(ms => ms.Value?.Errors.Count > 0)
                        .Select(ms => new
                        {
                            Field = ms.Key,
                            Errors = ms.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        });

                return BadRequest(new
                {
                    success = false,
                    message = "Invalid Food request",
                    errors
                });
            }

            var responce = await _foodService.UpdateAsync(food);

            return Ok(new
            {
                success = responce.Succeeded,
                data = responce,
                message = responce.Message + " " + string.Join(", ", responce.Errors),
            });
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteFoodView(int id)
        {
            var food = await _foodService.GetById(id);
            return View("DeleteFood", food);
        }


        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult > Delete(int foodId)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                        .Where(ms => ms.Value?.Errors.Count > 0)
                        .Select(ms => new
                        {
                            Field = ms.Key,
                            Errors = ms.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        });

                return BadRequest(new
                {
                    success = false,
                    message = "Invalid Food request",
                    errors
                });
            }

            var responce = await _foodService.DeleteAsync(new FoodDeleteViewModel { id = foodId });

            return Ok(new
            {
                success = responce.Succeeded,
                data = responce,
                message = responce.Message + " " + string.Join(", ", responce.Errors),
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var food = await _foodService.GetById(id);

            if(food == null)
                return NotFound( new { success = false , message = "Unsuccessful fetching of food item either it doesn't exist or server failers"});

            return Ok( new { success = true , data = food });
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
