using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace OAuthClient
{
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
            services.AddControllersWithViews();

            services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = "CookieScheme";

                config.DefaultSignInScheme = "CookieScheme";

                config.DefaultChallengeScheme = "OAuthScheme";
            })
            .AddCookie("CookieScheme")
            .AddOAuth("OAuthScheme", config =>
            {
                config.CallbackPath = "/oauth/callback"; // can customized to others

                config.ClientId = "oauthclient";

                config.AuthorizationEndpoint = "https://localhost:5001/oauth/authorize";

                config.TokenEndpoint = "https://localhost:5001/oauth/token";

                config.ClientSecret = "ClientSecret";

                config.SaveTokens = true;

                config.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents()
                {
                    OnCreatingTicket = cxt =>
                    {
                        var token = cxt.AccessToken;

                        var payload = token.Split(".")[1];

                        var mod = 4 - (payload.Length % 4);

                        var pad = string.Concat(Enumerable.Range(0, mod).Select(_ => "="));

                        var claims = JsonConvert
                        .DeserializeObject<Dictionary<string, string>>(
                            System.Text.Encoding.UTF8.GetString(
                                Convert.FromBase64String(payload + pad))
                        );

                        foreach (var claim in claims)
                        {
                            cxt.Identity.AddClaim(new System.Security.Claims.Claim(claim.Key, claim.Value));
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
