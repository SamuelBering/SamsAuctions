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

        }

        private Task<ClosedAuctionViewModel> CreateClosedAuctionViemModel(Auction auction)
        {

        }

        public async Task<IActionResult> GetAuctionDetails(int auctionId)
        {
            var auction = await _auctions.GetAuction(auctionId, groupCode);
            if (_auctions.isOpen(auction))
            {
                var model = await CreateOpenAuctionViemModel(auction);
                return PartialView("OpenAuctionDetails", model);
            }
            else
            {
                var model = await CreateClosedAuctionViemModel(auction);
                return PartialView("ClosedAuctionDetails", model);
            }
        }


    }
}
