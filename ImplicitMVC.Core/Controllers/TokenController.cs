using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImplicitMVC.Core.Controllers
{
    [Authorize]
    public class TokenController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}