using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SamsAuctions.BL;
using SamsAuctions.Infrastructure;
using SamsAuctions.Models;
using SamsAuctions.Models.ViewModels;
using SamsAuctions.Services;

namespace SamsAuctions.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StatisticsController : Controller
    {
        
        private IStatisticsService _statisticsService;
        private AppConfiguration _appConfiguration;
        private int groupCode;
        private UserManager<AppUser> _userManager;

        public StatisticsController(IStatisticsService statisticsService, AppConfiguration appConfiguration, UserManager<AppUser> userManager)
        {
            _statisticsService = statisticsService;
            _appConfiguration = appConfiguration;
            groupCode = appConfiguration.GroupCode;
            _userManager = userManager;
        }

        public async Task<IActionResult> GetAuctionsStatistics(GetStatisticsViewModel getStatisticsViewModel)
        {
            var viewModel = await _statisticsService.GetAuctionsStatistics(groupCode, getStatisticsViewModel.StartDate.Value,
                getStatisticsViewModel.EndDate.Value, User, getStatisticsViewModel.SelectedAuctionType == 1 ? false : true);
            return Ok(viewModel);
        }

        public IActionResult Index()
        {
            var currentDateTime = DateTime.UtcNow.AddHours(2);
            var StartDate = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, currentDateTime.Hour, currentDateTime.Minute, 0);

            var vm = new GetStatisticsViewModel
            {
                SelectedAuctionType = 1,
                StartDate = StartDate,
                EndDate = StartDate.AddMonths(1),
                AuctionTypes = new List<AuctionTypeViewModel>
                      {
                           new AuctionTypeViewModel {Id = 1, Type = "Alla auktioner"},
                           new AuctionTypeViewModel {Id = 2, Type = "Egna auktioner"}
                      }
            };
            return View(vm);
        }

    }
}