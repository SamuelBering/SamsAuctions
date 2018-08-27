using AutoMapper;
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
           .ForMember(a => a.Utropspris, b => b.MapFrom(c => c.ReservationPrice));
    }
}