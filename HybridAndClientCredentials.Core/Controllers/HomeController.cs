using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using HybridAndClientCredentials.Core.Configuration.Constants;

namespace HybridAndClientCredentials.Core.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Login()
        {
            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            return new SignOutResult(new[] { CookieAuthenticationDefaults.AuthenticationScheme, AuthenticationConstants.OidcAuthenticationScheme });
        }
    }
}
