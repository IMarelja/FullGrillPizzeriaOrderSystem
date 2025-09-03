using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Controllers
{
    
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> Index()
        {
            var user = await _userService.GetMeAsync();

            if (user == null) {
                return RedirectToAction("Login", "Authentication");
            }

            return View("Index", user);
        }

        [HttpPost]
        [Authorize(Policy = "UserOnly")]
        public IActionResult UpdateMe()
        {


            return View("Index");
        }
    }
}
