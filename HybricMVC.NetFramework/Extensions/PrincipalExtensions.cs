using IdentityModel;
using System.Security.Claims;

namespace HybricMVC.NetFramework.Extensions
{
    public static class PrincipalExtensions
    {
        public static string GetDisplayName(this ClaimsPrincipal principal)
        {
            var nameClaim = principal.FindFirst(JwtClaimTypes.Name);
            if (nameClaim != null) return nameClaim.Value;

            return string.Empty;
        }

        public static string GetPreferedDisplayName(this ClaimsPrincipal principal)
        {
            var nameClaim = principal.FindFirst(JwtClaimTypes.PreferredUserName);
            if (nameClaim != null) return nameClaim.Value;

            return string.Empty;
        }
    }
}
