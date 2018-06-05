using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HybridAndClientCredentials.Clasic.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task LogOut()
        {
           // await HttpContext.SignOutAsync();
        }
    }
}