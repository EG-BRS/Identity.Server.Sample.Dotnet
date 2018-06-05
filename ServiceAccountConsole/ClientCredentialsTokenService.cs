using System;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace ClientCredentialsConsole
{
    public class ClientCredentialsTokenService
    {
        private Credentials _credentials;
        
        private DiscoveryResponse _disco;

        public string RequestError { get; private set; }

        public bool IsValid => !_disco.IsError || _disco == null;

        public static async Task<ClientCredentialsTokenService> CreateAsync(Credentials cred)
        {
            return new ClientCredentialsTokenService()
            {
                _credentials = cred,
                _disco = await GetDiscoveryAsync(cred.IdentityEndPoint)
            };
        }

        private static async Task<DiscoveryResponse> GetDiscoveryAsync(string identityServerEndpoint)
        {
            var disco = await DiscoveryClient.GetAsync(identityServerEndpoint);

            if(disco.IsError) Console.WriteLine(disco.Error);

            return !disco.IsError ? disco : null;
        }

        public async Task<JObject> RequestToken()
        {
            RequestError = null;
            
            var tokenClient = new TokenClient(_disco.TokenEndpoint, _credentials.ClientId, _credentials.Secret);
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync(_credentials.Apis);

            if (!tokenResponse.IsError) return tokenResponse.Json;
            RequestError = tokenResponse.ErrorDescription;
            return null;
        }
    }
}