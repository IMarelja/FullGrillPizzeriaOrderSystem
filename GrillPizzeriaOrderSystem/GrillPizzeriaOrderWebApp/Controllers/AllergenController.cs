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

        [HttpGet]
        public IActionResult CreateDiagram()
        {
            return View("_CreateAllergenDiagram");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDropDown()
        {
            var response = await _allergenService.GetAll();

            return PartialView("_AllergenDropdown", response);
        }

        public IActionResult Create()
            => PartialView("_CreateAllergenDialog", new AllergenCreateViewModel());

        [HttpGet]
        public async Task<IActionResult> Create(AllergenCreateViewModel allergen)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateAllergenDialog", allergen);

            var response = await _allergenService.CreateAsync(allergen);

            return PartialView("_CreateAllergenDialog", response);
        }

        public IActionResult Delete()
            => PartialView("_DeleteAllergenDialog", 0);

        [HttpDelete]
        public async Task<IActionResult> Delete(int n)
        {
            var response = await _allergenService.DeleteAsync(n);
            return PartialView("_DeleteAllergenDialog", response);
        }
    }
}
