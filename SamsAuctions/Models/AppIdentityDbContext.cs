using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SamsAuctions.Models.ViewModels;
using System;
using System.Threading.Tasks;

namespace SamsAuctions.Models
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) { }

        public static async Task CreateDefaultRoles(IApplicationBuilder app,
                                                    IConfiguration configuration)
        {
            IServiceScopeFactory scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                
                // Seed database code goes here
                var serviceProvider = scope.ServiceProvider;

                UserManager<AppUser> userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
                RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var roles = configuration.GetSection("DefaultRoles").Get<string[]>();

                foreach (string role in roles)
                {
                    if (await roleManager.FindByNameAsync(role) == null)
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

           
        }

        public static async Task CreateDefaultUsers(IApplicationBuilder app, IConfiguration configuration)
        {
            IServiceScopeFactory scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                UserManager<AppUser> userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
                RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                //var adminUser = await userManager.FindByNameAsync("admin@nackowskis.se");
                //await userManager.AddToRoleAsync(adminUser, "Admin");

                var usersVM = configuration.GetSection("DefaultUsers").Get<DefaultUserViewModel[]>();

                foreach (var userVM in usersVM)
                {

                    if (await userManager.FindByNameAsync(userVM.Email) == null)
                    {

                        AppUser user = new AppUser
                        {
                            UserName = userVM.Email,
                            Email = userVM.Email,
                        };

                        IdentityResult result = await userManager.CreateAsync(user, userVM.Password);

                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, userVM.Role);
                        }
                    }
                }
            }
        }
       

    }
}
