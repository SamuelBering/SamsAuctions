using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SamsAuctions.BL;
using SamsAuctions.Infrastructure;
using SamsAuctions.Models;
using SamsAuctions.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamsAuctions.Controllers
{
    [Authorize]
    public class BidsController : Controller
    {
        private IAuctions _auctions;
        private AppConfiguration _appConfiguration;
        private int groupCode;

        public BidsController(IAuctions auctions, AppConfiguration appConfiguration)
        {
            _auctions = auctions;
            _appConfiguration = appConfiguration;
            groupCode = appConfiguration.GroupCode;
        }
        private async Task<OpenAuctionViewModel> CreateOpenAuctionViemModel(Auction auction)
        {
            var openAuctionViewModel = Mapper.Map<Auction, OpenAuctionViewModel>(auction);
            openAuctionViewModel.Bids = Mapper.Map<IList<Bid>, IList<BidViewModel>>(await _auctions.GetAllBids(auction.AuktionID, groupCode));

            return openAuctionViewModel;
        }

        private async Task<ClosedAuctionViewModel> CreateClosedAuctionViemModel(Auction auction)
        {
            var closedAuctionViewModel = Mapper.Map<Auction, ClosedAuctionViewModel>(auction);
            closedAuctionViewModel.highestBid = (await _auctions.GetWinningBid(auction)).Summa;
            return closedAuctionViewModel;
        }

        public async Task<IActionResult> GetClosedAuctionDetails(int auctionId)
        {
            var auction = await _auctions.GetAuction(auctionId, groupCode);
            var model = await CreateClosedAuctionViemModel(auction);
            return PartialView("ClosedAuctionDetails", model);
        }

        public async Task<IActionResult> GetOpenAuctionDetails(int auctionId)
        {
            var auction = await _auctions.GetAuction(auctionId, groupCode);
            var model = await CreateOpenAuctionViemModel(auction);
            return View("OpenAuctionDetails", model);  
        }


    }
}
