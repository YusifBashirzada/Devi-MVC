using Microsoft.AspNetCore.Mvc;

namespace Devi.Controllers
{
    public class PortfolioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
