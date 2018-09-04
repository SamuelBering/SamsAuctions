using SamsAuctions.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SamsAuctions.BL
{
    public interface IAuctions
    {
        Task<IList<Auction>> GetAllAuctions(int groupCode, ClaimsPrincipal user);
        Task<IList<Auction>> GetClosedAuctions(int groupCode, ClaimsPrincipal userClaimsPrincipal, bool ownAuctions = false);
        Task<IList<Auction>> GetClosedAuctions(int groupCode, DateTime startDate, DateTime endDate, ClaimsPrincipal userClaimsPrincipal, bool ownAuctions = false);
        Task AddOrUpdateAuction(Auction auction, ClaimsPrincipal user);
        Task AddBid(Bid bid, int groupCode);
        Task RemoveAuction(int auctionId, int groupCode, ClaimsPrincipal user);
        Task<Auction> GetAuction(int id, int groupCode);
        bool isOpen(Auction auction);
        Task<Bid> GetHighestBid(Auction auction);
        Task<Bid> GetWinningBid(Auction auction);
        Task<IList<Bid>> GetAllBids(int auctionId, int groupCode);
    }
}
