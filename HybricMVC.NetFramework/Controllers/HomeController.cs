using System.Web;
using System.Web.Mvc;

namespace HybricMVC.NetFramework.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Login()
        {
            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            // Signout from your app and xena login provider
            Request.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index", typeof(HomeController).Name);
        }
    }
}