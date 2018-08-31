using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SamsAuctions.BL;
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

        const int groupCode = 7;

        public BidsController(IAuctions auctions)
        {
            _auctions = auctions;
        }
        private Task<OpenAuctionViewModel> CreateOpenAuctionViemModel(Auction auction)
        {
            throw new NotImplementedException();
        }

        private async Task<ClosedAuctionViewModel> CreateClosedAuctionViemModel(Auction auction)
        {
            return new ClosedAuctionViewModel();

            
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
