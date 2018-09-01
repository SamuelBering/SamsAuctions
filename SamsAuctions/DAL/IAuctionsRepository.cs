using SamsAuctions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamsAuctions.DAL
{
    public interface IAuctionsRepository
    {
        Task AddOrUpdateAuction(Auction viewModel);
        Task<IList<Auction>> GetAllAuctions(int groupCode);
        Task<IList<Bid>> GetAllBids(int groupCode, int auctionId);
        Task RemoveAuction(int auctionId, int groupCode);
        Task<Auction> GetAuction(int id, int groupCode);
    }
}
