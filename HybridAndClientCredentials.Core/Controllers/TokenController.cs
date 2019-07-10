using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IdentityModel.Client;
using System.Net.Http;
using Microsoft.Extensions.Options;
using HybridAndClientCredentials.Core.Configuration;
using System.Collections.Generic;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace HybridAndClientCredentials.Core.Controllers
{
    [Authorize]
    public class TokenController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDiscoveryCache _discoveryCache;
        private readonly AuthConfiguration _authOptions;

        public TokenController(IHttpClientFactory httpClientFactory, IDiscoveryCache discoveryCache, IOptions<AuthConfiguration> authOptions)
        {
            _httpClientFactory = httpClientFactory;
            _discoveryCache = discoveryCache;
            _authOptions = authOptions.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Manual renew tokens
        /// Sends refresh token to IdentityServer to get new access and refresh token.
        /// </summary>
        /// <returns>Updated tokens</returns>
        public async Task<IActionResult> RenewTokens()
        {
            var disco = await _discoveryCache.GetAsync();
            if (disco.IsError) throw new Exception(disco.Error);

            var tokenClient = _httpClientFactory.CreateClient();
            var oldRefreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            var tokenResult = await tokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = _authOptions.ClientId,
                ClientSecret = _authOptions.ClientSecret,
                RefreshToken = oldRefreshToken
            });

            if (!tokenResult.IsError)
            {
                var idToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
                var newAccessToken = tokenResult.AccessToken;
                var newRefreshToken = tokenResult.RefreshToken;

                var tokens = new List<AuthenticationToken>
                        {
                            new AuthenticationToken { Name = OpenIdConnectParameterNames.IdToken, Value = idToken },
                            new AuthenticationToken { Name = OpenIdConnectParameterNames.AccessToken, Value = newAccessToken },
                            new AuthenticationToken { Name = OpenIdConnectParameterNames.RefreshToken, Value = newRefreshToken }
                        };

                AuthenticateResult info = await HttpContext.AuthenticateAsync();
                info.Properties.StoreTokens(tokens);
                await HttpContext.SignInAsync(info.Principal, info.Properties);

                return Redirect("~/Token/Index");
            }

            ViewData["Error"] = tokenResult.Error;
            return View("Error");
        }
    }
}