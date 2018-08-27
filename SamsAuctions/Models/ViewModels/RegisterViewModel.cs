using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SamsAuctions.Models.ViewModels
{
  
    public class RegisterViewModel
    {
        [Display(Name = "Förnamn")]
        [Required(ErrorMessage = "Förnamn är obligatoriskt")]
        public string FirstName { get; set; }

        [Display(Name = "Efternamn")]
        [Required(ErrorMessage = "Efternamn är obligatoriskt")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email är obligatoriskt")]
        [UIHint("email")]
        [EmailAddress(ErrorMessage = "Ogiltig email")]
        public string Email { get; set; }

        [UIHint("password")]
        [Display(Name = "Lösenord")]
        [Required(ErrorMessage = "Lösenord är obligatoriskt")]
        public string Password { get; set; }

        [UIHint("password")]
        [Display(Name = "Konfirmera nytt lösenord")]
        [Required(ErrorMessage = "Konfirmera lösenord är obligatoriskt")]
        [Compare(nameof(Password), ErrorMessage = "Lösenorden överrensstämmer inte")]
        public string ConfirmPassword { get; set; }
    }

}
