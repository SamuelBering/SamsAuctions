using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamsAuctions.Models.ViewModels
{
    public class AuctionsIndexViewModel
    {
        public AuctionsIndexViewModel(string sortOrder, string titleFilter, string descriptionFilter )
        {
            EndDateSortParam = String.IsNullOrEmpty(sortOrder) ? "endDate_desc" : "";
            ReservationPriceSortParam = sortOrder == "reservationPrice" ? "reservationPrice_desc" : "reservationPrice";
            TitleFilter = titleFilter;
            DescriptionFilter = descriptionFilter;
        }
        public AppUser CurrentUser { get; set; }
        public string EndDateSortParam { get; set; }
        public string ReservationPriceSortParam { get; set; }
        public string TitleFilter { get; set; }
        public string DescriptionFilter { get; set; }

        public List<AuctionViewModel> Auctions { get; set; }

    }
}
