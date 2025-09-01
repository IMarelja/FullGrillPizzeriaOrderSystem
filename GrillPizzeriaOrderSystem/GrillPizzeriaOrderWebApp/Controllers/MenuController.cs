using Microsoft.AspNetCore.Mvc;

namespace GrillPizzeriaOrderWebApp.Controllers
{
    public class MenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
