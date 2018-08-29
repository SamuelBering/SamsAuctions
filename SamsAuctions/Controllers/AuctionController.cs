using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SamsAuctions.BL;
using SamsAuctions.DAL;
using SamsAuctions.Models;
using SamsAuctions.Models.ViewModels;

namespace SamsAuctions.Controllers
{
    [Authorize]
    public class AuctionController : Controller
    {

        private IAuctions _auctions;

        const int groupCode = 7;

        public AuctionController(IAuctions auctions)
        {
            _auctions = auctions;
        }

        public async Task<IActionResult> Index(string sortOrder, string titleFilter, string descriptionFilter)
        {

            var model = new AuctionsIndexViewModel(sortOrder, titleFilter, descriptionFilter);

            var auctionViewModelList = Mapper.Map<IList<Auction>, IList<AuctionViewModel>>(await _auctions.GetAllAuctions(groupCode));

            model.Auctions = auctionViewModelList as List<AuctionViewModel>;


            if (!String.IsNullOrEmpty(titleFilter) || !String.IsNullOrEmpty(descriptionFilter))
            {
                titleFilter = titleFilter ?? "";
                descriptionFilter = descriptionFilter ?? "";
                model.Auctions = model.Auctions.Where(a => a.Title.Contains(titleFilter) && a.Description.Contains(descriptionFilter)).ToList();
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

        public async Task<IActionResult> Delete(int id)
        {
            //try
            //{
            await _auctions.RemoveAuction(id, groupCode, User);
            //}
            //catch (InvalidOperationException ex)
            //{

            //}

            var model = new AuctionsIndexViewModel(null, null, null);

            var auctionViewModelList = Mapper.Map<IList<Auction>, IList<AuctionViewModel>>(await _auctions.GetAllAuctions(groupCode));

            model.Auctions = auctionViewModelList as List<AuctionViewModel>;

            return View("Index", model);
        }
        //public IActionResult Index()
        //{

        //    return View();
        //}

        //public async Task<IActionResult> EditAuction(int? id)
        //{

        //    return View();
        //}

        public async Task<IActionResult> EditAuctionModal(int? id)
        {
            var auctionViewModel = new AuctionViewModel();

            if (id != null)
                auctionViewModel = Mapper.Map<Auction, AuctionViewModel>(await _auctions.GetAuction(id.Value, groupCode));

            return PartialView("EditAuction", auctionViewModel);
        }


        [HttpPost]
        [AllowAnonymous]
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