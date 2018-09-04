using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamsAuctions.Models.ViewModels
{
    public class AuctionsStatisticsViewModel
    {
        public AuctionsStatisticsViewModel()
        {
            Points = new List<string>();
            ReservationPrices = new List<int>();
            FinalPrices = new List<int>();
            Differences = new List<int>();
        }

        public IList<string> Points { get; set; }
        public IList<int> ReservationPrices { get; set; }
        public IList<int> FinalPrices { get; set; }
        public IList<int> Differences { get; set; }
    }
}
