using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HybridAndClientCredentials.Core.Controllers
{
    [Produces("application/json")]
    [Route("api/Secure")]
    public class SecureController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SecureController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        [Authorize]
        public async Task<IActionResult> Index(int clientId, string error)
        {
            ViewData["Title"] = "Secrets";
            
            if (User.Identity.IsAuthenticated)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
            }

            return View();
        }
    }
}