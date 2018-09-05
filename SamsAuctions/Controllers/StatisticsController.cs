using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SamsAuctions.BL;
using SamsAuctions.Infrastructure;
using SamsAuctions.Models;
using SamsAuctions.Models.ViewModels;

namespace SamsAuctions.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StatisticsController : Controller
    {
        private IAuctions _auctions;
        private AppConfiguration _appConfiguration;
        private int groupCode;
        private UserManager<AppUser> _userManager;

        public StatisticsController(IAuctions auctions, AppConfiguration appConfiguration, UserManager<AppUser> userManager)
        {
            _auctions = auctions;
            _appConfiguration = appConfiguration;
            groupCode = appConfiguration.GroupCode;
            _userManager = userManager;
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
                        viewModel.Points.Add($"({auctionTitelAbbr})");
                    }
                    else
                        viewModel.Points.Add($"{auction.SlutDatum.ToString("MMM", CultureInfo.CreateSpecificCulture("sv-SE"))} ({auctionTitelAbbr})");
                }
                else
                    viewModel.Points.Add($"{auction.SlutDatum.ToString("yyyy MMM", CultureInfo.CreateSpecificCulture("sv-SE"))} ({auctionTitelAbbr})");

                lastDate = auction.SlutDatum;
            }

            return viewModel;
        }

        public async Task<IActionResult> GetAuctionsStatistics(GetStatisticsViewModel getStatisticsViewModel)
        {
            var auctions = await _auctions.GetClosedAuctions(groupCode, User);
            var viewModel = await CreateAuctionsStatisticsViewModel(auctions);
            return Ok(viewModel);
        }

        public IActionResult Index()
        {
            var vm = new GetStatisticsViewModel
            {
                SelectedAuctionType=1,
                StartDate=DateTime.UtcNow.AddHours(2),
                EndDate = DateTime.UtcNow.AddHours(2).AddMonths(1),
                AuctionTypes = new List<AuctionTypeViewModel>
                      {
                           new AuctionTypeViewModel {Id = 1, Type = "Alla auktioner"},
                           new AuctionTypeViewModel {Id = 2, Type = "Egna auktioner"}
                      }
            };
            return View(vm);
        }

    }
}