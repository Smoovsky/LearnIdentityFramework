using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityExample.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleAuth.ClaimTransform;

namespace SimpleAuth
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
            services.AddAuthentication("SchemaCookie")
                .AddCookie("SchemaCookie", config =>
                {
                    config.Cookie.Name = "Auth";
                    config.LoginPath = "/Home/Authenticate";
                });

            services.AddAuthorization(config =>
            {
                // var defualtPolicyBuilder = new AuthorizationPolicyBuilder();

                // var defaultPolicy = defualtPolicyBuilder
                //     .RequireAuthenticatedUser() // this is the default behavior
                //     .RequireClaim(ClaimTypes.DateOfBirth)
                //     .Build();

                // config.DefaultPolicy = defaultPolicy;

                // try to implement this in a custom way
                // config.AddPolicy(
                //     "Claim.DOB",
                //     policyBuilder => 
                //     {
                //         policyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
                //     });

                config.AddPolicy(
                    "Claim.DOB",
                    policyBuilder =>
                    {
                        policyBuilder.AddRequirements(new CustomClaimRequirement(ClaimTypes.DateOfBirth));
                    });
            });

            services.AddControllersWithViews(config =>
            {
                // config.Filters.Add(new AuthorizeFilter());
            });

            services.AddScoped<IAuthorizationHandler, CustomClaimAuthHandler>();
            services.AddScoped<IAuthorizationHandler, SampleOperationAuthHandler>();
            services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, CustomPolicyProvider>();
            // services.AddRazorPages();

            services.AddRazorPages(config =>
            {
                config.Conventions.AuthorizePage("/Secured");
                config.Conventions.AuthorizePage("/Secured", "Claim.DOB"); // require policy here
                // config.Conventions.AuthorizeFolder("/Secured", "Claim.DOB"); 
                // config.Conventions.AllowAnonymousToPage("/Secured");
            });

            services.AddScoped<IClaimsTransformation, ClaimTransformer>();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();

            app.UseStaticFiles();

            // app.UseAuthorization();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
