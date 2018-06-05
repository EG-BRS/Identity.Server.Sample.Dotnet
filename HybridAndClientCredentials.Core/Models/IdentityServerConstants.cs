using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HybridAndClientCredentials.Core.Models
{
    public class IdentityServerConstants
    {
        public class AuthenticationHeader
        {
            public const string Bearer = "Bearer";
        }
        public class HttpContextHeaders
        {
            public const string AccessToken = "access_token";
            public const string RefreshToken = "refresh_token";
        }
    }
}
