namespace HybridAndClientCredentials.Core.Models
{
    public class Credentials
    {
        public string IdentityServerEndpoint { get; set; }
        public string ClientId { get; set; }
        public string IdToken { get; set; }
        public string Secret { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public void SetAccessToken(string accessToken)
        {
            this.AccessToken = accessToken;
        }

        public void SetRefreshToken(string refreshToken)
        {
            this.RefreshToken = refreshToken;
        }
    }
}