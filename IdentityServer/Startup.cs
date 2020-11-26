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
using IdentityServer4.Models;
using IdentityModel;
using IdentityServer4;
using IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>()
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource
            {
                Name = "trait.scope",
                UserClaims =
                {
                    "trait"
                }
            }
        };

        public static IEnumerable<ApiResource> GetApiResources() =>
            new List<ApiResource>()
            {
                new ApiResource("api1")
                {
                    Scopes = {"api1"}
                },
                new ApiResource("api2")
                {
                    Scopes = {"api2"}
                }
            };

        public static IEnumerable<ApiScope> GetApiScopes() =>
            new List<ApiScope>()
            {
                new ApiScope("api1"),
                new ApiScope("api2")
            };

        public static IEnumerable<Client> GetClients() =>
            new List<Client>()
            {
                new Client()
                {
                    ClientId = "client1",
                    ClientSecrets =
                    {
                        new Secret("client1secret".ToSha256())
                    },
                    AllowedGrantTypes =
                    {
                        GrantType.ClientCredentials
                    },
                    AllowedScopes = {"api1"}
                },
                new Client()
                {
                    ClientId = "clientMvc",
                    ClientSecrets =
                    {
                        new Secret("clientMvcSecret".ToSha256())
                    },
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowedScopes =
                    {
                        "api1",
                        "api2",
                        // "openid" // note: openid is an identity resource
                        IdentityServerConstants.StandardScopes.OpenId, // or this 
                        IdentityServerConstants.StandardScopes.Profile,
                        "trait.scope"
                    },
                    RedirectUris = new []{"https://localhost:9001/signin-oidc"},
                    RequireConsent = false,
                    // AlwaysIncludeUserClaimsInIdToken = true // active send claims
                }
            };
    }
    // https://localhost:5001/.well-known/openid-configuration
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

            services.AddDbContext<AppDbContext>(
                config => config.UseInMemoryDatabase("app")
            );

            services.AddIdentity<IdentityUser, IdentityRole>(
                config =>
                {
                    config.Password.RequiredLength = 4;
                    config.Password.RequireDigit = false;
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(
            config =>
            {
                config.Cookie.Name = "Identity.Cookie";
                config.LoginPath = "/auth/login";
            });

            services
            .AddIdentityServer()
            .AddAspNetIdentity<IdentityUser>()
            .AddInMemoryIdentityResources(Config.GetIdentityResources())
            .AddInMemoryApiScopes(Config.GetApiScopes())
            .AddInMemoryApiResources(Config.GetApiResources())
            .AddInMemoryClients(Config.GetClients())
            .AddDeveloperSigningCredential();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // if (env.IsDevelopment())
            // {
            app.UseDeveloperExceptionPage();
            // }
            // else
            // {
            //     app.UseExceptionHandler("/Home/Error");
            //     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //     app.UseHsts();
            // }
            // app.UseHttpsRedirection();
            // app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();

            // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
