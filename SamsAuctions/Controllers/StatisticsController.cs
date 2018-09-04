using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SamsAuctions.BL;
using SamsAuctions.Infrastructure;
using SamsAuctions.Models;

namespace SamsAuctions.Controllers
{
    public class StatisticsController : Controller
    {
        private IAuctions _auctions;
        private AppConfiguration _appConfiguration;
        private int groupCode;
        private UserManager<AppUser> _userManager;

        public StatisticsController(IAuctions auctions, AppConfiguration appConfiguration, UserManager<AppUser> userManager)
        {
            _auctions = auctions;
            _appConfiguration = appConfiguration;
            groupCode = appConfiguration.GroupCode;
            _userManager = userManager;
        }

        //private async Task<>
        public async Task<IActionResult> Index()
        {
            var auctions = await _auctions.GetClosedAuctions(groupCode, User);
            return View();
        }

    }
}