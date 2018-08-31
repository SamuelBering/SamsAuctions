using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamsAuctions.Models.ViewModels;
using SamsAuctions.Services;

namespace SamsAuctions.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private IUserRolesService userRolesService;

        public AdminController(IUserRolesService userRolesServ)
        {
            userRolesService = userRolesServ;
        }

        public async Task<IActionResult> Index()
        {
            var usersVM = await userRolesService.GetUsers();

            return View(usersVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserRole(UpdateUserRolesViewModel userVM)
        {
           
            if (ModelState.IsValid)
            {
                await userRolesService.UpdateUserRoles(userVM);
                return NoContent();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError,new { internalMessage="Modelstate is invalid!"});
            }  

        }
    }
}