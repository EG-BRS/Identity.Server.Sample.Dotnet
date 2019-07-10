using HybridAndClientCredentials.Core.Configuration.Interfaces;
using Microsoft.Extensions.Options;

namespace HybridAndClientCredentials.Core.Configuration
{
    public class RootConfiguration : IRootConfiguration
    {
        public IAuthConfiguration AuthConfiguration { get; }
        public IApiEndpoints ApiEndpoints { get; }

        public RootConfiguration(IOptions<AuthConfiguration> auth, IOptions<ApiEndpoints> api)
        {
            AuthConfiguration = auth.Value;
            ApiEndpoints = api.Value;
        }
    }
}
