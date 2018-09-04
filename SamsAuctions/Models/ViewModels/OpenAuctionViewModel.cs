using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SamsAuctions.Models.ViewModels
{
    public class OpenAuctionViewModel
    {
        [Display(Name = "Titel")]
        public string Title { get; set; }
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }
        [Display(Name = "Slutdatum")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Utropspris")]
        public int ReservationPrice { get; set; }

        public IList<BidViewModel> Bids { get; set; }
    }
}
