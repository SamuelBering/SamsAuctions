using SamsAuctions.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamsAuctions.DAL
{
    public interface IAuctionsRepository
    {
        Task AddOrUpdateAuction(AuctionViewModel viewModel);
    }
}
