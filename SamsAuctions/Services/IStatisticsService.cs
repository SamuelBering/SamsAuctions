using SamsAuctions.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SamsAuctions.Services
{
    public interface IStatisticsService
    {
        Task<AuctionsStatisticsViewModel> GetAuctionsStatistics(int groupCode, DateTime startDate, DateTime endDate, ClaimsPrincipal userClaimsPrincipal, bool ownAuctions = false);
    }
}
