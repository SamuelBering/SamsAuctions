using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SamsAuctions.Models.ViewModels
{
    public class AuctionViewModel
    {     
        public int AuctionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int GroupCode { get; set; }
        public int ReservationPrice { get; set; }
        public string CreatedBy { get; set; }
    }
}
