﻿using SamsAuctions.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SamsAuctions.BL
{
    public interface IAuctions
    {
        Task<IList<Auction>> GetAllAuctions(int groupCode, ClaimsPrincipal user);
        Task AddOrUpdateAuction(Auction auction, ClaimsPrincipal user);
        Task RemoveAuction(int auctionId, int groupCode, ClaimsPrincipal user);
        Task<Auction> GetAuction(int id, int groupCode);
        bool isOpen(Auction auction);
        Task<Bid> GetWinningBid(Auction auction);
    }
}
