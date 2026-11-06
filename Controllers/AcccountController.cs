using Microsoft.AspNetCore.Mvc;

namespace AgriStream.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}