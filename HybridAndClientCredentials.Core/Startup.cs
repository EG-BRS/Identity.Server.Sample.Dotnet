using IdentityModel.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using HybridAndClientCredentials.Core.Configuration.Constants;
using HybridAndClientCredentials.Core.Configuration;
using HybridAndClientCredentials.Core.Configuration.Interfaces;
using HybridAndClientCredentials.Core.Services;
using HybridAndClientCredentials.Core.Extensions;
using Microsoft.Extensions.Options;

namespace HybridAndClientCredentials.Core
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<AuthConfiguration>(Configuration.GetSection("AuthConfiguration"));
            services.Configure<ApiEndpoints>(Configuration.GetSection("ApiEndpoints"));
            services.AddTransient<IXenaService, XenaService>();
            var authConfiguration = services.BuildServiceProvider().GetService<IOptions<AuthConfiguration>>().Value;
            services.AddSingleton<IDiscoveryCache>(r =>
            {
                var factory = r.GetRequiredService<IHttpClientFactory>();
                return new DiscoveryCache(authConfiguration.Authority, () => factory.CreateClient());
            });

            services.AddMvc();
            services.AddHttpClient();
            services.AddHttpContextAccessor();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = AuthenticationConstants.OidcAuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(AuthenticationConstants.OidcAuthenticationScheme, options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.ResponseType = AuthenticationConstants.OidcResponseType;

                    options.Authority = authConfiguration.Authority;
                    options.ClientId = authConfiguration.ClientId;
                    options.ClientSecret = authConfiguration.ClientSecret;

                    options.SaveTokens = true;
                    options.RequireHttpsMetadata = false;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.Scope.Add("profile");
                    options.Scope.Add("testapi");
                    options.Scope.Add("offline_access");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();

            app.UseStaticFiles();

            // You can use middleware or implement automatic renew to cookie middleware
            app.UseAutomaticSilentRenew();
            app.UseAccessTokenLifetime();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
