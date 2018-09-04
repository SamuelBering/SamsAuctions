using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private UserManager<AppUser> _userManager;

        public BidsController(IAuctions auctions, AppConfiguration appConfiguration, UserManager<AppUser> userManager)
        {
            _auctions = auctions;
            _appConfiguration = appConfiguration;
            groupCode = appConfiguration.GroupCode;
            _userManager = userManager;
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
            closedAuctionViewModel.highestBid = (await _auctions.GetWinningBid(auction))?.Summa ?? 0;
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

        public async Task<IActionResult> BidModal(int auctionId)
        {
            var bidViewModel = new BidViewModel();
            bidViewModel.AuctionId = auctionId;
            bidViewModel.Bidder = (await _userManager.GetUserAsync(User)).UserName;
            return PartialView(bidViewModel);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyAmount(int amount, int auctionId)
        {
            var auction = await _auctions.GetAuction(auctionId, groupCode);
            var highestBid = await _auctions.GetHighestBid(auction);

            var highestBidAmount = highestBid?.Summa ?? 0;

            if (amount <= (highestBidAmount))
            {
                return Json(data: $"Du måste lägga ett bud som är högre än {highestBidAmount.ToString("C")}");
            }

            return Json(data: true);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bid(BidViewModel model)
        {
            if (ModelState.IsValid)
            {
                var bid = Mapper.Map<BidViewModel, Bid>(model);
                await _auctions.AddBid(bid, groupCode);
                var bidsVM = Mapper.Map<IList<Bid>, IList<BidViewModel>>(await _auctions.GetAllBids(bid.AuktionID, groupCode));
                return PartialView("Bids", bidsVM);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new { internalMessage = "Modelstate is invalid!" });
        }

    }
}
