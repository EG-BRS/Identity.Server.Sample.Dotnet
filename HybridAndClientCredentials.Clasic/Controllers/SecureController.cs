using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HybridAndClientCredentials.Clasic.Controllers
{
    public class SecureController : Controller
    {
        [Authorize]
        public async Task<ActionResult> Index(int clientId, string error)
        {
            ViewData["Title"] = "Secrets";

            return View();
        }
    }
}