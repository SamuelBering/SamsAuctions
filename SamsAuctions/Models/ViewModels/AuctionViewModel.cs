using System;
using System.ComponentModel.DataAnnotations;

namespace SamsAuctions.Models.ViewModels
{
    public class AuctionViewModel
    {     
        public int AuctionId { get; set; }
        [Required(ErrorMessage = "Förnamn är obligatoriskt")]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int GroupCode { get; set; }
        public int ReservationPrice { get; set; }
        public string CreatedBy { get; set; }
    }
}
