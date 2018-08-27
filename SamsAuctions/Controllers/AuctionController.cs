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

        public AuctionController(IAuctionsRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {

            return View();
        }

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