using Microsoft.AspNetCore.Mvc;

namespace GrillPizzeriaOrderMiddleware.Controllers
{
    [Route("api/menu")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        public MenuController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
