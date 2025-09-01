using System.Text.Json;
using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels;
using static System.Net.WebRequestMethods;

namespace GrillPizzeriaOrderWebApp.Controllers
{
    public class AllergenController : Controller
    {
        private readonly IAllergenService _allergenService;

        public AllergenController(IAllergenService allergenService)
        {
            _allergenService = allergenService;
        }

        public IActionResult Index()
        {
            return View();
        }
        /*
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
        */
    }
}
