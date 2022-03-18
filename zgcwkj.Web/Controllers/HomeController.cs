using Microsoft.AspNetCore.Mvc;

namespace zgcwkj.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
