using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using HybridAndClientCredentials.Core.Models;
using HybridAndClientCredentials.Core.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace HybridAndClientCredentials.Core
{

    public static class CredentialsBuilder
    {
        public static Credentials BuildCredentials(IConfiguration config)
        {
            var settings = config.GetSection("ServiceSettings");
            return new Credentials()
            {
                IdentityServerEndpoint = settings["IdentityServerEndpoint"],
                ClientId = settings["ClientId"],
                Secret = settings["Secret"]
            };
        }
    }


    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMvc();
            
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var cred = CredentialsBuilder.BuildCredentials(Configuration);
            services.AddSingleton(cred);

            XenaHttpClient.Initialize(Configuration["ServiceSettings:IdentityServerEndpoint"]);
            services.AddTransient<XenaHttpClient>();
            
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";

                    options.Authority = Configuration["ServiceSettings:IdentityServerEndpoint"];
                    options.RequireHttpsMetadata = false;

                    options.ClientId = Configuration["ServiceSettings:ClientId"];
                    options.ClientSecret = Configuration["ServiceSettings:secret"];
                    options.ResponseType = "code id_token";

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.Scope.Add("profile");
                    options.Scope.Add("testapi");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
