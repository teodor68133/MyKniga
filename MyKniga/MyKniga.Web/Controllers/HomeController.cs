using Microsoft.AspNetCore.Mvc;

namespace MyKniga.Web.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}