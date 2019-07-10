using System.Threading.Tasks;
using HybridAndClientCredentials.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HybridAndClientCredentials.Core.Controllers
{
    public class XenaController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IXenaService _xenaService;

        public XenaController(IConfiguration configuration, IXenaService xenaService)
        {
            _configuration = configuration;
            _xenaService = xenaService;
        }

        [Authorize]
        public async Task<IActionResult> CallXena()
        {
            var result = await _xenaService.GetUserFiscalSetupAsync();
            ViewBag.Json = result;
            return View("json");
        }

        [Authorize]
        public async Task<IActionResult> XenaMembership()
        {
            var result = await _xenaService.GetUserMembershipAsync();
            ViewBag.Json = result;
            return View("json");
        }

        [Authorize]
        public async Task<IActionResult> GetApps()
        {
            var fiscalId = HttpContext.User?.FindFirst("xena_fiscal_id")?.Value;
            var result = await _xenaService.GetFiscalXenaAppsAsync(fiscalId);
            ViewBag.Json = result;
            return View("json");
        }
    }
}
