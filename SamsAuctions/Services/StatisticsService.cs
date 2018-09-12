using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SamsAuctions.BL;
using SamsAuctions.Models;
using SamsAuctions.Models.ViewModels;
using System.Globalization;

namespace SamsAuctions.Services
{
    public class StatisticsService : IStatisticsService
    {
        private IAuctions _auctions;

        public StatisticsService(IAuctions auctions)
        {
            _auctions = auctions;
        }

        public async Task<AuctionsStatisticsViewModel> GetAuctionsStatistics(int groupCode, DateTime startDate, DateTime endDate, ClaimsPrincipal userClaimsPrincipal, bool ownAuctions = false)
        {
            var auctions = await _auctions.GetClosedAuctions(groupCode, startDate,
               endDate, userClaimsPrincipal, ownAuctions);
            var viewModel = await CreateAuctionsStatisticsViewModel(auctions);
            return viewModel;
        }

        private async Task<AuctionsStatisticsViewModel> CreateAuctionsStatisticsViewModel(IList<Auction> auctions)
        {
            var viewModel = new AuctionsStatisticsViewModel();

            auctions = auctions.OrderBy(a => a.SlutDatum).ToList();

            var lastDate = new DateTime(1000, 1, 1);

            foreach (var auction in auctions)
            {
                var winningBid = await _auctions.GetWinningBid(auction);
                if (winningBid == null)
                    continue;

                viewModel.ReservationPrices.Add(auction.Utropspris);
                viewModel.FinalPrices.Add(winningBid.Summa);
                viewModel.Differences.Add(winningBid.Summa - auction.Utropspris);
                var auctionTitelAbbr = auction.Titel.Length > 6 ? auction.Titel.Substring(0, 6) : auction.Titel;

                if (auction.SlutDatum.Year == lastDate.Year)
                {
                    if (auction.SlutDatum.Month == lastDate.Month)
                    {
                        viewModel.Points.Add($"{auctionTitelAbbr}");
                    }
                    else
                        viewModel.Points.Add($"{auction.SlutDatum.ToString("MMM", CultureInfo.CreateSpecificCulture("sv-SE")).ToUpper()} - {auctionTitelAbbr}");
                }
                else
                    viewModel.Points.Add($"{auction.SlutDatum.ToString("yyyy MMM", CultureInfo.CreateSpecificCulture("sv-SE")).ToUpper()} - {auctionTitelAbbr}");

                lastDate = auction.SlutDatum;
            }

            return viewModel;
        }

    }
}
