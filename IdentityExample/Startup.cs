using IdentityExample.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityExample
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

            services.AddDbContext<AppDbContext>(
                config => config.UseInMemoryDatabase("app")
            );

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // services.AddIdentity<IdentityUser, IdentityRole>(c =>
            // {
            //     c.Password.RequiredLength = 4;
            //     c.Password.RequireDigit = false;
            //     c.Password.RequireNonAlphanumeric = false;
            //     c.Password.RequireUppercase = false;
            // })
            // .AddEntityFrameworkStores<AppDbContext>()
            // .AddDefaultTokenProviders(); // another example to configure password requirement

            services.Configure<IdentityOptions>(c =>
            {
                c.Password.RequiredLength = 4;
                c.Password.RequireDigit = false;
                c.Password.RequireNonAlphanumeric = false;
                c.Password.RequireUppercase = false; // test if this works
                c.Password.RequireLowercase = false; // test if this works
            });

            services.ConfigureApplicationCookie(
                config =>
                {
                    config.Cookie.Name = "Identity.Cookie";
                    config.LoginPath = "/home/login";
                });
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
