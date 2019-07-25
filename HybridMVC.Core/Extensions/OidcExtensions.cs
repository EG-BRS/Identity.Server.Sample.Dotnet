using HybridMVC.Core.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace HybridMVC.Core.Extensions
{
    public static class OidcExtensions
    {
        public static IApplicationBuilder UseAutomaticSilentRenew(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AutomaticSilentRenewMiddleware>();
        }

        public static IApplicationBuilder UseAccessTokenLifetime(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenLifetimeMiddleware>(OpenIdConnectParameterNames.AccessToken);
        }
    }
}
