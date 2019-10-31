using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

[assembly: OwinStartup(typeof(HybricMVC.NetFramework.Startup))]
namespace HybricMVC.NetFramework
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //_log.Debug("Startup");
            var identityServerUrl = "https://localhost:44343/";//ConfigurationManager.AppSettings["IdentityServerUrl"];
            var basePath = "http://localhost:64832";//ConfigurationManager.AppSettings["BasePath"];
            var secret = "Secret";//ConfigurationManager.AppSettings["IdSClientSecret"];
            var clientId = "Hybrid";// ConfigurationManager.AppSettings["IdSClientId"];
            //app.Use<XForwardedMiddleware>();

            if (!string.IsNullOrWhiteSpace(identityServerUrl))
            {
                //app.Use(async (context, next) =>
                //{
                //    if (string.IsNullOrWhiteSpace(context.Request.Headers["Authorization"]))
                //    {
                //        if (context.Request.QueryString.HasValue)
                //        {
                //            var token = context.Request.Query["Access_Token"];
                //            if (!string.IsNullOrWhiteSpace(token))
                //            {
                //                context.Request.Headers.Add("Authorization", new[] { $"Bearer {token}" });
                //            }
                //        }
                //    }
                //    await next.Invoke();
                //});
                app.UseCookieAuthentication(new CookieAuthenticationOptions());
                app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
                {
                    Authority = identityServerUrl,
                    ClientId = clientId,
                    Scope = "openid profile testapi offline_access",
                    ClientSecret = secret,
                    RedirectUri = basePath + "/signin-oidc",
                    PostLogoutRedirectUri = basePath,
                    ResponseType = "id_token token code",
                    SignInAsAuthenticationType = "Cookies",
                    
                    //RequireHttpsMetadata = false,
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