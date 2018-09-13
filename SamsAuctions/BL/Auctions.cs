using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SamsAuctions.DAL;
using SamsAuctions.Models;

namespace SamsAuctions.BL
{
    public class Auctions : IAuctions
    {
        IAuctionsRepository _repository;
        private UserManager<AppUser> _userManager;


        public Auctions(IAuctionsRepository repository, UserManager<AppUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }


        public async Task AddOrUpdateAuction(Auction auction, ClaimsPrincipal user)
        {

            if (user?.IsInRole("Admin") ?? false)
            {
                var currentUserName = (await _userManager.GetUserAsync(user)).UserName;

                if (auction.AuktionID == 0 || (auction.AuktionID > 0 && currentUserName == auction.SkapadAv))
                    await _repository.AddOrUpdateAuction(auction);
                else
                    throw new InvalidOperationException("User must have created this auction");
            }
            else
                throw new InvalidOperationException("User must be in role: Admin");
        }
        public async Task<IList<Auction>> GetClosedAuctions(int groupCode, ClaimsPrincipal userClaimsPrincipal, bool ownAuctions = false)
        {
            return await GetClosedAuctions(groupCode, new DateTime(1000, 1, 1), new DateTime(4000, 1, 1), userClaimsPrincipal, ownAuctions);
        }

        public async Task<IList<Auction>> GetClosedAuctions(int groupCode, DateTime startDate, DateTime endDate, ClaimsPrincipal userClaimsPrincipal, bool ownAuctions = false)
        {
            var user = await _userManager.GetUserAsync(userClaimsPrincipal);
            var auctions = (await _repository.GetAllAuctions(groupCode))
                .Where((a) =>
                {
                    var result = false;

                    if (!isOpen(a) && a.SlutDatum >= startDate && a.SlutDatum <= endDate)
                        result = true;

                    if (ownAuctions)
                    {
                        if (a.SkapadAv != user.UserName)
                            result = false;
                    }

                    return result;
                });

            return auctions.ToList();
        }

        public async Task<IList<Auction>> GetAllAuctions(int groupCode, ClaimsPrincipal userClaimsPrincipal)
        {
            var auctions = await _repository.GetAllAuctions(groupCode);

            var user = await _userManager.GetUserAsync(userClaimsPrincipal);

            foreach (var auction in auctions)
            {
                auction.AnvandarenFarTaBort = false;

                if (auction.SkapadAv == user.UserName && userClaimsPrincipal.IsInRole("Admin"))
                {
                    auction.AnvandarenFarUppdatera = true;
                    var allBids = await _repository.GetAllBids(groupCode, auction.AuktionID);
                    if (allBids.Count == 0)
                        auction.AnvandarenFarTaBort = true;
                }
                else
                    auction.AnvandarenFarUppdatera = false;

                if (isOpen(auction))
                    auction.ArOppen = true;
                else
                    auction.ArOppen = false;

            }

            return auctions;
        }

        public async Task<IList<Bid>> GetAllBids(int auctionId, int groupCode)
        {
            return await _repository.GetAllBids(groupCode, auctionId);
        }

        public async Task<Auction> GetAuction(int id, int groupCode)
        {
            return await _repository.GetAuction(id, groupCode);
        }

        public async Task AddBid(Bid bid, int groupCode)
        {
            var auction = await _repository.GetAuction(bid.AuktionID, groupCode);
            var highestBid = await GetHighestBid(auction);
            if (bid.Summa <= (highestBid?.Summa ?? 0))
                throw new InvalidOperationException("Bid must be the highest bid");
            else
                await _repository.AddBid(bid);
        }

        public async Task<Bid> GetHighestBid(Auction auction)
        {
            var bids = await _repository.GetAllBids(auction.Gruppkod, auction.AuktionID);

            if (bids.Count == 0)
                return null;

            var highestBid = bids.OrderByDescending(b => b.Summa).First();
            return highestBid;
        }

        public async Task<Bid> GetWinningBid(Auction auction)
        {
            if (isOpen(auction))
                throw new InvalidOperationException("This auction is still open");

            return await GetHighestBid(auction);
        }

        public bool isOpen(Auction auction)
        {
            if (auction.SlutDatum > DateTime.UtcNow.AddHours(2))
                return true;
            else
                return false;
        }

        public async Task RemoveAuction(int auctionId, int groupCode, ClaimsPrincipal user)
        {
            if (user?.IsInRole("Admin") ?? false)
            {

                var currentUserName = (await _userManager.GetUserAsync(user)).UserName;
                var auction = await GetAuction(auctionId, groupCode);

                if (currentUserName == auction.SkapadAv)
                {
                    var allBids = await _repository.GetAllBids(groupCode, auctionId);
                    if (allBids.Count == 0)
                        await _repository.RemoveAuction(auctionId, groupCode);
                    else
                        throw new InvalidOperationException("Can´t remove an auction that contains bids");
                }
                else
                    throw new InvalidOperationException("User must have created this auction");
            }
            else
                throw new InvalidOperationException("User must be in role: Admin");
        }
    }
}
