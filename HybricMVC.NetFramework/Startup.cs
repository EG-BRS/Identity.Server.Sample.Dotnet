using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.Configuration;
using System.Linq;
using System.Security.Claims;

[assembly: OwinStartup(typeof(HybricMVC.NetFramework.Startup))]
namespace HybricMVC.NetFramework
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var identityServerUrl = ConfigurationManager.AppSettings["IdentityServerUrl"];
            var basePath = ConfigurationManager.AppSettings["ApplicationUrl"];
            var secret = ConfigurationManager.AppSettings["Secret"];
            var clientId = ConfigurationManager.AppSettings["ClientId"];

            if (!string.IsNullOrWhiteSpace(identityServerUrl))
            {
                app.UseCookieAuthentication(new CookieAuthenticationOptions());
                app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
                {
                    Authority = identityServerUrl,
                    ClientId = clientId,
                    Scope = "openid profile testapi",
                    ClientSecret = secret,
                    RedirectUri = basePath + "/signin-oidc",
                    PostLogoutRedirectUri = basePath,
                    ResponseType = "id_token token code",
                    SignInAsAuthenticationType = "Cookies",

                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        SecurityTokenValidated = async n =>
                        {
                            // Adding tokens to claims
                            var id = n.AuthenticationTicket.Identity;
                            id.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));
                            id.AddClaim(new Claim("code", n.ProtocolMessage.Code));
                            id.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                        },
                        RedirectToIdentityProvider = async n =>
                        {
                            if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                            {
                                var idTokenClaim = n.OwinContext.Authentication.User.Claims.SingleOrDefault(x => x.Type == "id_token");
                                if (idTokenClaim != null)
                                {
                                    n.ProtocolMessage.IdTokenHint = idTokenClaim.Value;
                                }
                            }
                        }
                    }
                });
            }
        }
    }
}