using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using GrillPizzeriaOrderWebApp.Models;
using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _env;
        public FoodController(IWebHostEnvironment env, IFoodService foodService)
        {
            _foodService = foodService;
            _env = env;
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

            if (food.imageUpload is { Length: > 0 })
            {
                var ext = Path.GetExtension(food.imageUpload.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                if (!allowed.Contains(ext))
                    return BadRequest(new { success = false, message = "Unsupported image format." });

                if (food.imageUpload.Length > 5 * 1024 * 1024)
                    return BadRequest(new { success = false, message = "File size exceeds 5MB limit." });

                var fileName = $"{Guid.NewGuid():N}{ext}";
                var imagesRoot = Path.Combine(_env.WebRootPath, "images");
                Directory.CreateDirectory(imagesRoot);
                var fullPath = Path.Combine(imagesRoot, fileName);

                try
                {
                    await using var stream = System.IO.File.Create(fullPath);
                    await food.imageUpload.CopyToAsync(stream);
                    food.imagePath = $"/images/{fileName}";
                }
                catch
                {
                    return BadRequest(new { success = false, message = "Failed to save image." });
                }
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

            if (!food.defaultImageKeep)
            {
                var imagePath = Path.Combine(_env.WebRootPath, food.imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(imagePath))
                {
                    try
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    catch
                    {

                    }
                }
            }

            if (food.imageUpload is { Length: > 0 })
            {
                var ext = Path.GetExtension(food.imageUpload.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                if (!allowed.Contains(ext))
                    return BadRequest(new { success = false, message = "Unsupported image format." });

                if (food.imageUpload.Length > 5 * 1024 * 1024)
                    return BadRequest(new { success = false, message = "File size exceeds 5MB limit." });

                var fileName = $"{Guid.NewGuid():N}{ext}";
                var imagesRoot = Path.Combine(_env.WebRootPath, "images");
                Directory.CreateDirectory(imagesRoot);
                var fullPath = Path.Combine(imagesRoot, fileName);

                try
                {
                    await using var stream = System.IO.File.Create(fullPath);
                    await food.imageUpload.CopyToAsync(stream);
                    food.imagePath = $"/images/{fileName}";
                }
                catch
                {
                    return BadRequest(new { success = false, message = "Failed to save image." });
                }
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

            var reponceFood = await _foodService.GetById(foodId);

            var responce = await _foodService.DeleteAsync(new FoodDeleteViewModel { id = foodId });

            if(responce.Succeeded && reponceFood != null && !string.IsNullOrEmpty(reponceFood.imagePath))
            {
                var imagePath = Path.Combine(_env.WebRootPath, reponceFood.imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(imagePath))
                {
                    try
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    catch
                    {

                    }
                }
            }

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
