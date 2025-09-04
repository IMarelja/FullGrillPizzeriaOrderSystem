using System.Reflection;
using System.Text.Json;
using GrillPizzeriaOrderWebApp.Models;
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

        [Authorize(Policy = "AdminOnly")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(AllergenCreateViewModel allergen)
        {
            if (!ModelState.IsValid)
            {
                var message = ApiOperationResult.Fail("Creation validation error.");
                return View("~/Views/Allergen/Index.cshtml", message);
            }

            var response = await _allergenService.CreateAsync(allergen);

            return View("~/Views/Allergen/Index.cshtml", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(AllergenEditViewModel allergen)
        {
            if (!ModelState.IsValid)
            {
                var message = ApiOperationResult.Fail("Editing validation error.");
                return View("~/Views/Allergen/Index.cshtml", message);
            }

            var response = await _allergenService.UpdateAsync(allergen);

            return View("~/Views/Allergen/Index.cshtml", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(AllergenDeleteViewModel allergen)
        {
            if (!ModelState.IsValid)
            {
                var message = ApiOperationResult.Fail("Deletion validation error.");
                return View("~/Views/Allergen/Index.cshtml", message);
            }

            var response = await _allergenService.DeleteAsync(allergen);

            return View("~/Views/Allergen/Index.cshtml", response);
        }

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allergens = await _allergenService.GetAll();

                return Ok(new
                {
                    success = true,
                    data = allergens,
                    message = "Food categories loaded successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Error loading food categories: " + ex.Message
                });
            }
        }
    }
}
