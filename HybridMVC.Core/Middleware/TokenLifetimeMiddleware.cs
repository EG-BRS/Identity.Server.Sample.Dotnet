using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace HybridMVC.Core.Middleware
{
    /// <summary>
    /// Sign out user if Access token is expired
    /// </summary>
    public class TokenLifetimeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _tokenName;
        private readonly string _cookieSchemeName = CookieAuthenticationDefaults.AuthenticationScheme;

        public TokenLifetimeMiddleware(RequestDelegate next, string tokenName)
        {
            _next = next;
            _tokenName = tokenName;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string token = await context.GetTokenAsync(_tokenName);
            if (!string.IsNullOrEmpty(token))
            {
                DateTime validTo = new JwtSecurityToken(token).ValidTo;
                if (validTo < DateTime.UtcNow)
                {
                    // Sign out if token is no longer valid
                    await context.SignOutAsync(_cookieSchemeName);
                }
            }

            await _next.Invoke(context);
        }
    }
}
