using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using SamsAuctions.Models;
using SamsAuctions.Models.ViewModels;

namespace SamsAuctions.Services
{
    public class UserRolesService : IUserRolesService
    {
        private UserManager<AppUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public UserRolesService(UserManager<AppUser> usrMgr, RoleManager<IdentityRole> roleMgr)
        {
            userManager = usrMgr;
            roleManager = roleMgr;
        }

        public async Task<ICollection<UpdateUserRolesViewModel>> GetUsers()
        {
            var roles = roleManager.Roles.ToList();

            List<UpdateUserRolesViewModel> usersVM = new List<UpdateUserRolesViewModel>();

            foreach (var user in userManager.Users)
            {

                UpdateUserRolesViewModel userVM = new UpdateUserRolesViewModel
                {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Roles = new List<SelectListItem>()
                };

                foreach (var role in roles)
                {
                    if (await userManager.IsInRoleAsync(user, role.Name))
                        userVM.Roles.Add(new SelectListItem
                        {
                            Value = role.Id,
                            Text = role.Name,
                            Selected = true
                        });
                    else
                        userVM.Roles.Add(new SelectListItem
                        {
                            Value = role.Id,
                            Text = role.Name,
                        });
                }

                usersVM.Add(userVM);
            }
            return usersVM;
        }

        public async Task UpdateUserRoles(UpdateUserRolesViewModel userVM)
        {
            var appUser = await userManager.FindByIdAsync(userVM.UserId);

            var roles = await userManager.GetRolesAsync(appUser);

            if (roles != null && roles.Count > 0)
                await userManager.RemoveFromRolesAsync(appUser, roles);

            foreach (var roleId in userVM.SelectedRoleIds)
            {
                var roleName = (await roleManager.FindByIdAsync(roleId)).Name;
                await userManager.AddToRoleAsync(appUser, roleName);
            }
        }


    }
}
