using ImplicitMVC.Core.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ImplicitMVC.Core.Extensions
{
    public static class OidcExtensions
    {
        public static IApplicationBuilder UseAccessTokenLifetime(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenLifetimeMiddleware>(OpenIdConnectParameterNames.AccessToken);
        }
    }
}
