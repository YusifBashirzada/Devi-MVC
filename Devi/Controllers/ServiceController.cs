using Microsoft.AspNetCore.Mvc;

namespace Devi.Controllers
{
    public class ServiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
