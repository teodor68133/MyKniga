using Microsoft.AspNetCore.Mvc;

namespace MyKniga.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}