using Microsoft.AspNetCore.Mvc;

namespace Sylvester_Thalia_HW5.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}