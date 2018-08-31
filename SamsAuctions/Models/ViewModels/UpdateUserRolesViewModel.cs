using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SamsAuctions.Models.ViewModels
{
    public class UpdateUserRolesViewModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "Du måste ange minst en roll")]
        public string[] SelectedRoleIds { get; set; }
        public List<SelectListItem> Roles { get; set; }
    }
}
