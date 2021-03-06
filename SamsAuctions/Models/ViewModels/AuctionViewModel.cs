﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SamsAuctions.Models.ViewModels
{
    public class AuctionViewModel : IValidatableObject
    {
        public int AuctionId { get; set; }
        [Display(Name = "Titel")]
        [MinLength(3, ErrorMessage = "Titeln måste vara minst tre tecken")]
        [Required(ErrorMessage = "Titel är obligatoriskt")]
        public string Title { get; set; }
        [Display(Name = "Beskrivning")]
        [Required(ErrorMessage = "Beskrivning är obligatoriskt")]
        public string Description { get; set; }
        [Display(Name = "Startdatum")]
        [Required(ErrorMessage = "Startdatum är obligatoriskt")]
        public DateTime StartDate { get; set; }
        [Display(Name = "Slutdatum")]
        [Required(ErrorMessage = "Slutdatatum är obligatoriskt")]
        public DateTime EndDate { get; set; }
        public int GroupCode { get; set; }
        [Display(Name = "Utropspris")]
        [Required(ErrorMessage = "Utropspris är obligatoriskt")]
        [Range(1, 5000000, ErrorMessage = "Utropspriset måste vara minst 1 sek")]
        public int ReservationPrice { get; set; }
        [Display(Name = "Skapad av")]
        [Required(ErrorMessage = "Skapad av är obligatoriskt")]
        public string CreatedBy { get; set; }
        public bool UserAllowedToUpdate { get; set; }
        public bool UserAllowedToRemove { get; set; }
        public bool IsOpen { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate <= StartDate)
            {
                yield return
                         new ValidationResult(errorMessage: "Slutdatum måste vara senare än startdatum",
                                              memberNames: new[] { "EndDate" });
            }
        }
    }
}
