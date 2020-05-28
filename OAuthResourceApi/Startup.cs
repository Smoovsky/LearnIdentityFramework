using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OAuthResourceApi
{
    public class JwtRequirement : IAuthorizationRequirement
    {

    }

    public class JwtHandler : AuthorizationHandler<JwtRequirement>
    {
        private readonly HttpClient _httpClient;

        private readonly HttpContext _httpContext;

        public JwtHandler(
            IHttpClientFactory clientFactory,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _httpClient = clientFactory.CreateClient();
            _httpContext = httpContextAccessor.HttpContext;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            JwtRequirement requirement)
        {
            if (_httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var accessToken = authHeader.ToString().Split(" ")[1];

                var validateRes = _httpClient.GetAsync($"https://localhost:5001/home/ValidateToken?jwt={accessToken}").Result;

                if(validateRes.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
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
            services.AddAuthentication("DefaultAuth")
                .AddScheme<AuthenticationSchemeoption, >;

            services.AddAuthorization(config =>
            {
                var defualtPolicyBuilder = new AuthorizationPolicyBuilder();

                var defaultPolicy = defualtPolicyBuilder
                    // .RequireAuthenticatedUser()
                    // .
                    .AddRequirements(new JwtRequirement())
                    .Build();

                config.DefaultPolicy = defaultPolicy;
            });

            services.AddControllersWithViews();

            services
            .AddHttpClient()
            .AddHttpContextAccessor();

            services
            .AddScoped<IAuthorizationHandler, JwtHandler>();
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
