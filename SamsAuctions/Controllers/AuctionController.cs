using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SamsAuctions.DAL;
using SamsAuctions.Models.ViewModels;

namespace SamsAuctions.Controllers
{
    [Authorize]
    public class AuctionController : Controller
    {
        private IAuctionsRepository _repository;

        const int groupCode= 7;

        public AuctionController(IAuctionsRepository repository)
        {
            _repository = repository;
        }

       
       
        public async Task<IActionResult> Index(string sortOrder, string titleFilter, string descriptionFilter)
        {

            var model = new AuctionsIndexViewModel(sortOrder, titleFilter, descriptionFilter);

            model.Auctions = await _repository.GetAllAuctions(groupCode);

          
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

        //public IActionResult Index()
        //{

        //    return View();
        //}

        public async Task<IActionResult> EditAuction(int? id)
        {
          
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAuction(AuctionViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                await _repository.AddOrUpdateAuction(model);

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