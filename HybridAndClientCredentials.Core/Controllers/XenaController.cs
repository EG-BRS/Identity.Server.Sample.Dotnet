using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HybridAndClientCredentials.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace HybridAndClientCredentials.Core.Controllers
{
    [Authorize]
    public class XenaController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IXenaService _xenaService;

        public XenaController(IConfiguration configuration, IXenaService xenaService)
        {
            _configuration = configuration;
            _xenaService = xenaService;
        }

        public async Task<IActionResult> UserFiscalSetup()
        {
            var result = await _xenaService.GetUserFiscalSetupAsync();
            ViewBag.Json = result;
            return View("json");
        }

        public async Task<IActionResult> UserMembership()
        {
            var result = await _xenaService.GetUserMembershipAsync();
            ViewBag.Json = result;
            return View("json");
        }

        public async Task<IActionResult> UserApps()
        {
            // Get fiscal setup for current user
            var fiscalSetup = await _xenaService.GetUserFiscalSetupAsync();
            // Get fiscal ids from fiscal setup
            dynamic obj = JObject.Parse(fiscalSetup);
            var fiscalTasks = new List<Task<string>>();
            // Get each app data from xena api
            foreach (var entity in obj.Entities)
                fiscalTasks.Add(_xenaService.GetFiscalXenaAppsAsync(entity.Id.ToString()));
            await Task.WhenAll(fiscalTasks);
            ViewBag.Json = string.Join("****", fiscalTasks.Select(x => x.Result));
            return View("json");
        }
    }
}
