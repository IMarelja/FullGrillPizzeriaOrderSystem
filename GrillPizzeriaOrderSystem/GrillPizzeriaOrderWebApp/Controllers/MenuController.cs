using System.Net.Http;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Index()
        {
            var client = _http.CreateClient("DataAPI");

            List<FoodViewModel> foodList;
            List<CategoryFoodViewModel> categoryFoodList;

            try
            {
                var responceFoodAPI = await client.GetAsync("Food");
                var responceFoodCategoryAPI = await client.GetAsync("FoodCategory");

                if (!responceFoodAPI.IsSuccessStatusCode || !responceFoodCategoryAPI.IsSuccessStatusCode)
                    throw new Exception();

                using var FoodJsonDocument = JsonDocument.Parse(await responceFoodAPI.Content.ReadAsStringAsync());

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

                using var FoodCategoryJsonDocument = JsonDocument.Parse(await responceFoodCategoryAPI.Content.ReadAsStringAsync());

                categoryFoodList = new List<CategoryFoodViewModel>();
                foreach (var element in FoodCategoryJsonDocument.RootElement.EnumerateArray())
                {
                    var foodCategory = new CategoryFoodViewModel
                    {
                        Id = element.GetProperty("id").GetInt32(),
                        Name = element.GetProperty("name").GetString() ?? string.Empty,
                    };
                    categoryFoodList.Add(foodCategory);
                }

            }
            catch
            {
                categoryFoodList = new List<CategoryFoodViewModel>();
                foodList = new List<FoodViewModel>();
            }

            ViewBag.Categories = categoryFoodList;
            ViewBag.Foods = foodList;

            return View();
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string? q, int? categoryId)
        {
            var client = _http.CreateClient("DataAPI");
            var qs = new Dictionary<string, string?>();

            if (!string.IsNullOrWhiteSpace(q)) qs["q"] = q;
            if (categoryId.HasValue) qs["categoryId"] = categoryId.Value.ToString();

            var url = QueryHelpers.AddQueryString("api/Food/search", qs); // -> http://.../api/Food/search?q=...&categoryId=...
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var foods = await response.Content.ReadFromJsonAsync<List<FoodViewModel>>();
            return PartialView("_FoodList", foods); // returns HTML snippet
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
