﻿using AutoMapper;
using SamsAuctions.Models;
using SamsAuctions.Models.ViewModels;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AuctionViewModel, Auction>()
           .ForMember(a => a.AuktionID, b => b.MapFrom(c => c.AuctionId))
           .ForMember(a => a.Beskrivning, b => b.MapFrom(c => c.Description))
            .ForMember(a => a.Gruppkod, b => b.MapFrom(c => c.GroupCode))
           .ForMember(a => a.SkapadAv, b => b.MapFrom(c => c.CreatedBy))
            .ForMember(a => a.SlutDatum, b => b.MapFrom(c => c.EndDate))
           .ForMember(a => a.StartDatum, b => b.MapFrom(c => c.StartDate))
            .ForMember(a => a.Titel, b => b.MapFrom(c => c.Title))
           .ForMember(a => a.Utropspris, b => b.MapFrom(c => c.ReservationPrice))
           .ForMember(a => a.AnvandarenFarUppdatera, b => b.MapFrom(c => c.UserAllowedToUpdate))
            .ForMember(a => a.AnvandarenFarTaBort, b => b.MapFrom(c => c.UserAllowedToRemove))
           .ForMember(a => a.ArOppen, b => b.MapFrom(c => c.IsOpen));

        CreateMap<Auction, AuctionViewModel>()
          .ForMember(a => a.AuctionId, b => b.MapFrom(c => c.AuktionID))
          .ForMember(a => a.Description, b => b.MapFrom(c => c.Beskrivning))
           .ForMember(a => a.GroupCode, b => b.MapFrom(c => c.Gruppkod))
          .ForMember(a => a.CreatedBy, b => b.MapFrom(c => c.SkapadAv))
           .ForMember(a => a.EndDate, b => b.MapFrom(c => c.SlutDatum))
          .ForMember(a => a.StartDate, b => b.MapFrom(c => c.StartDatum))
           .ForMember(a => a.Title, b => b.MapFrom(c => c.Titel))
          .ForMember(a => a.ReservationPrice, b => b.MapFrom(c => c.Utropspris))
          .ForMember(a => a.UserAllowedToUpdate, b => b.MapFrom(c => c.AnvandarenFarUppdatera))
           .ForMember(a => a.UserAllowedToRemove, b => b.MapFrom(c => c.AnvandarenFarTaBort))
          .ForMember(a => a.IsOpen, b => b.MapFrom(c => c.ArOppen));

        CreateMap<Auction, ClosedAuctionViewModel>()
          .ForMember(a => a.Title, b => b.MapFrom(c => c.Titel))
          .ForMember(a => a.ReservationPrice, b => b.MapFrom(c => c.Utropspris))
          .ForMember(a => a.EndDate, b => b.MapFrom(c => c.SlutDatum))
          .ForMember(a => a.Description, b => b.MapFrom(c => c.Beskrivning))
          .ForMember(a => a.CreatedBy, b => b.MapFrom(c => c.SkapadAv));

        CreateMap<Auction, OpenAuctionViewModel>()
          .ForMember(a => a.AuctionId, b => b.MapFrom(c => c.AuktionID))
          .ForMember(a => a.Title, b => b.MapFrom(c => c.Titel))
          .ForMember(a => a.ReservationPrice, b => b.MapFrom(c => c.Utropspris))
          .ForMember(a => a.EndDate, b => b.MapFrom(c => c.SlutDatum))
          .ForMember(a => a.Description, b => b.MapFrom(c => c.Beskrivning));

        CreateMap<Bid, BidViewModel>()
             .ForMember(a => a.AuctionId, b => b.MapFrom(c => c.AuktionID))
             .ForMember(a => a.Bidder, b => b.MapFrom(c => c.Budgivare))
             .ForMember(a => a.BidId, b => b.MapFrom(c => c.BudID))
             .ForMember(a => a.Amount, b => b.MapFrom(c => c.Summa));

        CreateMap<BidViewModel, Bid>()
            .ForMember(a => a.AuktionID, b => b.MapFrom(c => c.AuctionId))
            .ForMember(a => a.Budgivare, b => b.MapFrom(c => c.Bidder))
            .ForMember(a => a.BudID, b => b.MapFrom(c => c.BidId))
            .ForMember(a => a.Summa, b => b.MapFrom(c => c.Amount));

    }
}