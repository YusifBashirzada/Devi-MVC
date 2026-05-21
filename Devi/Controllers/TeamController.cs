using Microsoft.AspNetCore.Mvc;

namespace Devi.Controllers
{
    public class TeamController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
