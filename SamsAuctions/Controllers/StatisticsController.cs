using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SamsAuctions.BL;
using SamsAuctions.Infrastructure;
using SamsAuctions.Models;
using SamsAuctions.Models.ViewModels;

namespace SamsAuctions.Controllers
{
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

                if (auction.SlutDatum.Year==lastDate.Year)
                {
                    if (auction.SlutDatum.Month == lastDate.Month)
                    {
                        viewModel.Points.Add("");
                    }
                    else
                        viewModel.Points.Add(auction.SlutDatum.ToString("MMM", CultureInfo.CreateSpecificCulture("sv-SE")));
                }
                else
                    viewModel.Points.Add(auction.SlutDatum.ToString("yyyy MMM", CultureInfo.CreateSpecificCulture("sv-SE")));

                lastDate = auction.SlutDatum;
            }

            return viewModel;
        }

        public async Task<IActionResult> GetAuctionsStatistics()
        {
            var auctions = await _auctions.GetClosedAuctions(groupCode, User);
            var viewModel = await CreateAuctionsStatisticsViewModel(auctions);
            return Ok(viewModel);
        }

        public IActionResult Index()
        {
            return View();
        }

    }
}