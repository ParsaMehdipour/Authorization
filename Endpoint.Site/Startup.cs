using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Endpoint.Site.Models.Context;
using Endpoint.Site.Repositories;
using Endpoint.Site.Security.Default;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersianTranslation.Identity;

namespace Endpoint.Site
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

            services.AddTransient<IMessageSender, MessageSender>();

            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("AppConnectionString"));
            });

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.Password.RequiredUniqueChars = 0;

                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-";

                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                })
                .AddEntityFrameworkStores<ApplicationContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<PersianIdentityErrorDescriber>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CarListPolicy", policy =>
                {
                    policy.RequireClaim(ClaimTypesStore.CarList, true.ToString(), false.ToString());
                });

                options.AddPolicy("AddCarPolicy", policy =>
                {
                    policy.RequireClaim(ClaimTypesStore.AddCar, true.ToString(), false.ToString());
                });

                options.AddPolicy("ClaimOrRole",policy =>
                {
                    policy.RequireAssertion(ClaimOrRole);
                });

                options.AddPolicy("ClaimRequirement", policy =>
                {
                    policy.AddRequirements(new ClaimRequirement(ClaimTypesStore.CarList, true.ToString()));
                });
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public bool ClaimOrRole(AuthorizationHandlerContext context)
        {
            return context.User.HasClaim(ClaimTypesStore.CarList, true.ToString()) ||
                context.User.IsInRole("Admin");
        }
    }
}
