using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SamsAuctions.BL;
using SamsAuctions.DAL;
using SamsAuctions.Infrastructure;
using SamsAuctions.Models;
using SamsAuctions.Models.ViewModels;

namespace SamsAuctions.Controllers
{
    [Authorize]
    public class AuctionController : Controller
    {

        private IAuctions _auctions;
        private UserManager<AppUser> _userManager;
        private AppConfiguration _appConfiguration;

        private int groupCode;

        public AuctionController(IAuctions auctions, UserManager<AppUser> userManager, AppConfiguration appConfiguration)
        {
            _auctions = auctions;
            _userManager = userManager;
            _appConfiguration = appConfiguration;
            groupCode = appConfiguration.GroupCode;
        }

        public async Task<IActionResult> Index(string sortOrder, string titleFilter, string descriptionFilter)
        {

            var model = new AuctionsIndexViewModel(sortOrder, titleFilter, descriptionFilter);

            var auctionViewModelList = Mapper.Map<IList<Auction>, IList<AuctionViewModel>>(await _auctions.GetAllAuctions(groupCode, User));

            model.Auctions = auctionViewModelList as List<AuctionViewModel>;

            model.CurrentUser = await _userManager.GetUserAsync(User);

            if (!String.IsNullOrEmpty(titleFilter) || !String.IsNullOrEmpty(descriptionFilter))
            {
                titleFilter = titleFilter ?? "";
                descriptionFilter = descriptionFilter ?? "";
                model.Auctions = model.Auctions.Where(a => a.Title.ToLower().Contains(titleFilter.ToLower()) && a.Description.ToLower().Contains(descriptionFilter.ToLower())).ToList();
            }

            switch (sortOrder)
            {
                case "endDate_desc":
                    model.Auctions = model.Auctions.OrderByDescending(a => a.EndDate).ToList();
                    break;
                case "reservationPrice":
                    model.Auctions = model.Auctions.OrderBy(a => a.ReservationPrice).ToList();
                    break;
                case "reservationPrice_desc":
                    model.Auctions = model.Auctions.OrderByDescending(a => a.ReservationPrice).ToList();
                    break;
                default:
                    model.Auctions = model.Auctions.OrderBy(a => a.EndDate).ToList();
                    break;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            //try
            //{
            await _auctions.RemoveAuction(id, groupCode, User);
            //}
            //catch (InvalidOperationException ex)
            //{

            //}

            //var model = new AuctionsIndexViewModel(null, null, null);

            //var auctionViewModelList = Mapper.Map<IList<Auction>, IList<AuctionViewModel>>(await _auctions.GetAllAuctions(groupCode));

            //model.Auctions = auctionViewModelList as List<AuctionViewModel>;

            //return View("Index", model);
            return RedirectToAction("Index");
        }
       

        public async Task<IActionResult> EditAuctionModal(int? id)
        {
            var auctionViewModel = new AuctionViewModel();
            auctionViewModel.GroupCode = groupCode;
            auctionViewModel.CreatedBy = (await _userManager.GetUserAsync(User)).UserName;

            if (id != null)
                auctionViewModel = Mapper.Map<Auction, AuctionViewModel>(await _auctions.GetAuction(id.Value, groupCode));

            return PartialView("EditAuction", auctionViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAuction(AuctionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var auction = Mapper.Map<AuctionViewModel, Auction>(model);

                await _auctions.AddOrUpdateAuction(auction, User);

                return RedirectToAction("Index");
            }
            //await _repository.AddOrUpdateAuction(new AuctionViewModel
            //{
            //    CreatedBy = "BadAss",
            //    Description = "A good gun",
            //    StartDate = new DateTime(2018, 08, 10),
            //    EndDate = new DateTime(2018, 12, 24),
            //    GroupCode = 7,
            //    ReservationPrice = 4400,
            //    Title = "Machine gun",
            //});

            return View(model);
        }
    }
}