using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HybridAndClientCredentials.Core.Extensions
{
    public static class PrincipalExtensions
    {
        public static string GetDisplayName(this ClaimsPrincipal principal)
        {
            var nameClaim = principal.FindFirst(JwtClaimTypes.Name);
            if (nameClaim != null) return nameClaim.Value;

            return string.Empty;
        }
    }
}
