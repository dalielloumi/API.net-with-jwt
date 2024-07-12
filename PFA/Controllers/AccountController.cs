using Microsoft.AspNetCore.Mvc;

namespace PFA.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
