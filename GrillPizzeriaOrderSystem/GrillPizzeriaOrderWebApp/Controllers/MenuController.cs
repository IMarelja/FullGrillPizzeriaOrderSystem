using System.Net.Http;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Controllers
{
    public class MenuController : Controller
    {
        private readonly IHttpClientFactory _http;
        public MenuController(IHttpClientFactory http) => _http = http;


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string search, int? foodCategoryId)
        {
            var client = _http.CreateClient("DataAPI");

            List<FoodViewModel> foodList;
            List<CategoryFoodViewModel> usedCategoryFoodList;

            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(search))
                queryParams.Add($"q={Uri.EscapeDataString(search)}");
            if (foodCategoryId.HasValue)
                queryParams.Add($"categoryId={foodCategoryId.Value}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;

            try
            {
                var responseFoodAPI = await client.GetAsync($"Food/search{queryString}");

                var tempCategoryList = new List<CategoryFoodViewModel>();

                if (!responseFoodAPI.IsSuccessStatusCode)
                    throw new Exception();

                using var FoodJsonDocument = JsonDocument.Parse(await responseFoodAPI.Content.ReadAsStringAsync());

                foodList = new List<FoodViewModel>();
                usedCategoryFoodList = new List<CategoryFoodViewModel>();
                foreach (var element in FoodJsonDocument.RootElement.EnumerateArray())
                {
                    var category = new CategoryFoodViewModel
                    {
                        Id = element.GetProperty("category").GetProperty("id").GetInt32(),
                        Name = element.GetProperty("category").GetProperty("name").GetString() ?? string.Empty
                    };

                    var food = new FoodViewModel
                    {
                        Id = element.GetProperty("id").GetInt32(),
                        Name = element.GetProperty("name").GetString() ?? string.Empty,
                        Description = element.GetProperty("description").GetString() ?? string.Empty,
                        Price = element.GetProperty("price").GetDecimal(),
                        ImagePath = element.GetProperty("imagePath").GetString() ?? string.Empty,
                        FoodCategoryId = element.GetProperty("foodCategoryId").GetInt32(),
                        Category = category,
                        Allergens = element.GetProperty("allergens")
                           .EnumerateArray()
                           .Select(a => new AllergenViewModel
                           {
                               id = a.GetProperty("id").GetInt32(),
                               name = a.GetProperty("name").GetString() ?? string.Empty
                           })
                           .ToList()
                    };
                    foodList.Add(food);

                    tempCategoryList.Add(category);

                    usedCategoryFoodList = tempCategoryList
                        .DistinctBy(c => c.Id)
                        .ToList();
                }

            }
            catch
            {
                usedCategoryFoodList = new List<CategoryFoodViewModel>();
                foodList = new List<FoodViewModel>();
            }

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategoryId = foodCategoryId;
            ViewBag.UsedCategories = new SelectList(
                usedCategoryFoodList,
                "Id",
                "Name",
                foodCategoryId
            );

            return View(foodList);
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> FoodSearch(string? search, int? foodCategoryId)
        {
            var client = _http.CreateClient("DataAPI");

            List<FoodViewModel> foodList;

            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(search))
                queryParams.Add($"q={Uri.EscapeDataString(search)}");
            if (foodCategoryId.HasValue)
                queryParams.Add($"categoryId={foodCategoryId.Value}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;

            try
            {
                var responseFoodAPI = await client.GetAsync($"Food/search{queryString}");

                var tempCategoryList = new List<CategoryFoodViewModel>();

                if (!responseFoodAPI.IsSuccessStatusCode)
                    throw new Exception();

                using var FoodJsonDocument = JsonDocument.Parse(await responseFoodAPI.Content.ReadAsStringAsync());

                foodList = new List<FoodViewModel>();
                foreach (var element in FoodJsonDocument.RootElement.EnumerateArray())
                {

                    var food = new FoodViewModel
                    {
                        Id = element.GetProperty("id").GetInt32(),
                        Name = element.GetProperty("name").GetString() ?? string.Empty,
                        Description = element.GetProperty("description").GetString() ?? string.Empty,
                        Price = element.GetProperty("price").GetDecimal(),
                        ImagePath = element.GetProperty("imagePath").GetString() ?? string.Empty,
                        FoodCategoryId = element.GetProperty("foodCategoryId").GetInt32(),
                        Category = new CategoryFoodViewModel
                        {
                            Id = element.GetProperty("category").GetProperty("id").GetInt32(),
                            Name = element.GetProperty("category").GetProperty("name").GetString() ?? string.Empty
                        },
                        Allergens = element.GetProperty("allergens")
                           .EnumerateArray()
                           .Select(a => new AllergenViewModel
                           {
                               id = a.GetProperty("id").GetInt32(),
                               name = a.GetProperty("name").GetString() ?? string.Empty
                           })
                           .ToList()
                    };
                    foodList.Add(food);
                }

            }
            catch
            {
                foodList = new List<FoodViewModel>();
            }

            return PartialView("_FoodListGrid", foodList);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllergens()
        {
            var client = _http.CreateClient("DataAPI");

            var responceAllergenAPI = await client.GetAsync("Allergen");

            try
            {
                

                if (!responceAllergenAPI.IsSuccessStatusCode)
                    return Json(new { error = "Failed to fetch allergens" });

                var content = await responceAllergenAPI.Content.ReadAsStringAsync();
                var allergens = JsonSerializer.Deserialize<List<AllergenViewModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return Json(allergens);

                
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpDelete]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteAllergen(int id)
        {
            var client = _http.CreateClient("DataAPI");
            var responseAllergenAPI = await client.DeleteAsync($"Allergen/{id}");
            try
            {
                if (!responseAllergenAPI.IsSuccessStatusCode)
                    return Json(new { success = false, error = "Failed to delete allergen" });

                return Json(new { success = true, message = "Allergen deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
