using System.ComponentModel.DataAnnotations;

namespace SamsAuctions.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email är obligatoriskt")]
        [EmailAddress(ErrorMessage = "Ogiltig email")]
        [UIHint("email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Lösenord är obligatoriskt")]
        [UIHint("password")]
        public string Password { get; set; }
    }
}
