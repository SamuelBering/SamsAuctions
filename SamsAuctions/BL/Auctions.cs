using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SamsAuctions.DAL;
using SamsAuctions.Models;

namespace SamsAuctions.BL
{
    public class Auctions : IAuctions
    {
        IAuctionsRepository _repository;

        public Auctions(IAuctionsRepository repository)
        {
            _repository = repository;
        }

        public async Task AddOrUpdateAuction(Auction auction, ClaimsPrincipal user)
        {

            if (user?.IsInRole("Admin") ?? false)
            {
                await _repository.AddOrUpdateAuction(auction);
            }
            else
                throw new InvalidOperationException("User must be in role: Admin");
        }

        public async Task<IList<Auction>> GetAllAuctions(int groupCode)
        {
            return await _repository.GetAllAuctions(groupCode);
        }

        public async Task<Auction> GetAuction(int id, int groupCode)
        {
            return await _repository.GetAuction(id, groupCode);
        }

        public async Task RemoveAuction(int auctionId, int groupCode, ClaimsPrincipal user)
        {
            if (user?.IsInRole("Admin") ?? false)
            {

                await _repository.RemoveAuction(auctionId, groupCode);
            }
            else
                throw new InvalidOperationException("User must be in role: Admin");
        }
    }
}
