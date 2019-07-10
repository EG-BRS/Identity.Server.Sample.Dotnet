using HybridAndClientCredentials.Core.Configuration.Interfaces;

namespace HybridAndClientCredentials.Core.Configuration
{
    public class AuthConfiguration : IAuthConfiguration
    {
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
