﻿using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace HybridAndClientCredentials.Core.Middleware
{
    public class AutomaticSilentRenewMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _cookieSchemeName;
        private readonly IDiscoveryCache _discoveryCache;

        public AutomaticSilentRenewMiddleware(IDiscoveryCache discoveryCache, RequestDelegate next, string clientId, string clientSecret, string cookieSchemeName)
        {
            _next = next;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _cookieSchemeName = cookieSchemeName;
            _discoveryCache = discoveryCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string oldAccessToken = await context.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if (!string.IsNullOrEmpty(oldAccessToken))
            {
                JwtSecurityToken tokenInfo = new JwtSecurityToken(oldAccessToken);

                // Renew access token if pass halfway of its lifetime
                if (tokenInfo.ValidFrom + (tokenInfo.ValidTo - tokenInfo.ValidFrom) / 2 < DateTime.UtcNow)
                {
                    var disco = await _discoveryCache.GetAsync();

                    TokenClient tokenClient = new TokenClient(disco.TokenEndpoint, _clientId, _clientSecret);
                    string oldRefreshToken = await context.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
                    TokenResponse tokenResult = await tokenClient.RequestRefreshTokenAsync(oldRefreshToken);

                    if (!tokenResult.IsError)
                    {
                        string idToken = await context.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
                        string newAccessToken = tokenResult.AccessToken;
                        string newRefreshToken = tokenResult.RefreshToken;

                        var tokens = new List<AuthenticationToken>
                        {
                            new AuthenticationToken { Name = OpenIdConnectParameterNames.IdToken, Value = idToken },
                            new AuthenticationToken { Name = OpenIdConnectParameterNames.AccessToken, Value = newAccessToken },
                            new AuthenticationToken { Name = OpenIdConnectParameterNames.RefreshToken, Value = newRefreshToken }
                        };

                        AuthenticateResult info = await context.AuthenticateAsync(_cookieSchemeName);
                        info.Properties.StoreTokens(tokens);
                        await context.SignInAsync(_cookieSchemeName, info.Principal, info.Properties);
                    }
                }
            }

            await _next.Invoke(context);
        }
    }
}
