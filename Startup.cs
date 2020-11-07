using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using esp2.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using esp2.Helper;

namespace esp2
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("Memory");
            });

            /*
             * AddIdentity method 其實是 Register Authentication Schema 的 method
             * 這個 function 幫你 register Authentication with 以下配置
             *  1. Use Cookie way for authentication
             *  2. Use IdentityUser Database for storing the user's claims(information)
             *  3. The user 's information maybe encrpt. For instance, the user's password. Addtion to that, 
             *     it provide a management system for managing user 's information(UserManager service, etc.)
             *  4. Take a attention, when it comes to sign in, you still need to take care of creating 
             *     claim principal things
             */
            services.AddIdentity<IdentityUser, IdentityRole>(options => {
                        
                    })
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options => {
                options.Cookie.Name = "Grandpa.Cookie";
                options.LoginPath = "/Home/Login";
            });

            services.AddScoped<
                IUserClaimsPrincipalFactory<IdentityUser>,
                SelfDefineUserClaimsPrincipalFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
