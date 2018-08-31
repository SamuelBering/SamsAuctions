using SamsAuctions.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamsAuctions.Services
{
    public interface IUserRolesService
    {
        Task<ICollection<UpdateUserRolesViewModel>> GetUsers();

        Task UpdateUserRoles(UpdateUserRolesViewModel user);
    }
}
