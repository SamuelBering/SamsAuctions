using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SamsAuctions.Models.ViewModels
{
    public class ClosedAuctionViewModel
    {
        [Display(Name = "Titel")]
        public string Title { get; set; }
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }
        [Display(Name = "Slutdatum")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Utropspris")]
        public int ReservationPrice { get; set; }
        [Display(Name = "Högsta/vinnande bud")]
        public int highestBid { get; set; }
        [Display(Name = "Skapad av")]
        public string CreatedBy { get; set; }
    }
}
