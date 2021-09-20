using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace HybricMVC.NetFramework.Controllers
{
    [Authorize]
    public class TokenController : Controller
    {
        public ActionResult Index()
        {
            return View((System.Security.Claims.ClaimsPrincipal)User);
        }
    }
}