using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SamsAuctions.Models.ViewModels
{
    public class GetStatisticsViewModel
    {
       
        [Display(Name = "Startdatum")]
        //[Required(ErrorMessage = "Belopp är obligatoriskt")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "Slutdatum")]
        public DateTime? EndDate { get; set; }
        public IEnumerable<AuctionTypeViewModel> AuctionTypes { set; get; }
        [Display(Name = "")]
        public int SelectedAuctionType { set; get; }
    }

}
