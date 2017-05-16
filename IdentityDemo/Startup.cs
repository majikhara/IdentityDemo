using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.Services;

namespace IdentityDemo
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            

            //eallain:  Enable Account lockout for protecting against brute force attacks
            /*
             * Recommendations to user account lockout with 2FA (two-factor authentication). 
             * Once a user logs in (through local account or social account), each failed attempt at 2FA is stored,
             * and if the maximum attempts (default is 5) is reached, the user is locked out for five minutes
             * you can set  the lockout time wiht DefaultAccountLockoutTimeSpan)
             * 
             * The following configures account lockout to be locked for 5 minutes after 3 failed attempts.
             *              * 
             */

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true; //this was done within the AccountController login post
                config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //this is the default
                config.Lockout.MaxFailedAccessAttempts = 3;  //this is not the default (5)


            });


            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            //eallain:  Register Custom Services
            services.AddTransient<AdministratorSeedData>();
            services.AddSingleton<IRequestFormDataService, RequestFormDataService>();

            /*
             * Singleton lifetime services are creaated the first time they are requested
             * (or when ConfigureServices is run if you specify the instance there) and
             * then every subsequent request will user the same instance.             * 
             */ 

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory,
            //eallain seeding database
            AdministratorSeedData seeder)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //eallain:  Seed administrator data
            await seeder.EnsureSeedData();  //if it needs to be awaited, method needs to be asynchronous(async)
        }
    }
}
