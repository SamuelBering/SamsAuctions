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

        public async Task<IList<Auction>> GetAllAuctions(int groupCode, ClaimsPrincipal userClaimsPrincipal)
        {
            var auctions= await _repository.GetAllAuctions(groupCode);

            var user = await _userManager.GetUserAsync(userClaimsPrincipal);

            foreach (var auction in auctions)
            {
                if (auction.SkapadAv == user.UserName)
                    auction.AnvandarenFarUppdatera = true;
                else
                    auction.AnvandarenFarUppdatera = false;

                if (isOpen(auction))
                    auction.ArOppen = true;
                else
                    auction.ArOppen = false;

            }

            return auctions;
        }

        public async Task<Auction> GetAuction(int id, int groupCode)
        {
            return await _repository.GetAuction(id, groupCode);
        }

        public async Task<Bid> GetWinningBid(Auction auction)
        {
            if (isOpen(auction))
                throw new InvalidOperationException("This auction is still open");

            var bids = await _repository.GetAllBids(auction.Gruppkod, auction.AuktionID);

            var highestBid = bids.OrderByDescending(b => b.Summa).First();

            return highestBid;
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
                    await _repository.RemoveAuction(auctionId, groupCode);
                else
                    throw new InvalidOperationException("User must have created this auction");
            }
            else
                throw new InvalidOperationException("User must be in role: Admin");
        }
    }
}
