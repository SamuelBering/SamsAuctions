﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SamsAuctions.Models.ViewModels
{
    public class BidViewModel
    {
        public int BidId { get; set; }
        [Display(Name = "Belopp")]     
        [Required(ErrorMessage = "Belopp är obligatoriskt")]
        [Range(1, 5000000, ErrorMessage = "Belopp måste vara minst 1 kr")]
        [Remote(action: "VerifyAmount", controller: "Bids", AdditionalFields = nameof(AuctionId))]
        public int Amount { get; set; }        
        public int AuctionId { get; set; }
        public string Bidder { get; set; }
    }
}
