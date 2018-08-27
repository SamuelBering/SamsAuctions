using System.ComponentModel.DataAnnotations;

namespace SamsAuctions.Models.ViewModels
{
    public class AssociateAccountViewModel
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
    }
}
