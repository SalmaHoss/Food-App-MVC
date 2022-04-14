//using FoodApp.Areas.Identity.Data;
using FoodApp.Data.Cart;
using FoodApp.Models;
using FoodApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodApp.Data;

namespace FoodApp
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
            services.AddDbContext<AppDbContext>(options=>
                     options.UseSqlServer(Configuration.GetConnectionString("myconn")));

            // services.AddDbContext<UserDbContext>(options =>
            //          options.UseSqlServer(Configuration.GetConnectionString("UserDbContextConnection")));

            //services.AddDefaultIdentity<AppUser>(options =>
            //         options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<UserDbContext>();

            services.AddRazorPages();
            services.AddScoped<IRestaurantRepsitory, RestaurantRepoService>();
            services.AddScoped<IItemRepository, ItemRepoService>();
            services.AddScoped<IOrderRepository, OrderRepoService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(sc => ShoppingCart.GetShoppingCart(sc));

            //Auth and Autherization

            /*
             addIdentity 2 parms:
            1-user class 
            2-Role
            then define where we want store all auth data
             */
            services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            {
                    o.SignIn.RequireConfirmedEmail = true;

                    o.Password.RequiredLength = 8;
                    o.Password.RequireNonAlphanumeric = true;
                    o.Password.RequireUppercase = true;
                    o.Password.RequireLowercase = false;

            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            //Takes action delegate as parameter
     
            
            services.AddMemoryCache();
            services.AddSession();

            //add authentication and define using Cookie in options
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            //add external logins
            services.AddAuthentication()
                .AddGoogle(o =>
                {
                    o.ClientId = "162887081599-8urbpvt42if135k780252g03sfh4bu2m.apps.googleusercontent.com";
                    o.ClientSecret = "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";
                })
                .AddFacebook(o =>
                {
                    o.AppId = "1123745748470433";
                    o.AppSecret = "404ba181b770660be894b1c84b01f5d8";
                });

            services.AddControllersWithViews();

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
            app.UseSession();

            //Authentication And Authorization
            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Restaurant}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });
            AppDbInitializer.SeedUsersAndRolesAsync(app).Wait();
        }
    }
}
