using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SamsAuctions.BL;
using SamsAuctions.DAL;
using SamsAuctions.Infrastructure;
using SamsAuctions.Models;
using SamsAuctions.Services;

namespace SamsAuctions
{
    public class Startup
    {
        public Startup(IConfiguration configuration) //Dependency injection of config object for reading appsettings.json
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            int groupCode = Convert.ToInt32(Configuration["AppConfiguration:GroupCode"]);

            services.AddSingleton(new AppConfiguration { GroupCode = groupCode });

            var connNackowskisIdentity = Configuration["ConnectionStrings:NackowskisIdentity"];

            services.AddDbContext<AppIdentityDbContext>(options =>
                                options.UseSqlServer(connNackowskisIdentity));

            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppIdentityDbContext>()
                    .AddDefaultTokenProviders();

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            });

            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(20);//You can set Time
            });

            services.AddTransient<IAuctionsRepository, AuctionsRepository>();
            services.AddTransient<IAuctions, Auctions>();
            services.AddTransient<IStatisticsService, StatisticsService>();
            services.AddTransient<IUserRolesService, UserRolesService>();

            services.AddAutoMapper();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=home}/{action=index}/{id?}"
                    );
            });


            AppIdentityDbContext.CreateDefaultRoles(app, Configuration).Wait();
            AppIdentityDbContext.CreateDefaultUsers(app, Configuration).Wait();
        }
    }
}
