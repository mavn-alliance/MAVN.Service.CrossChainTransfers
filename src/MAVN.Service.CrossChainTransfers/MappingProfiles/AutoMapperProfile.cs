using AutoMapper;
using Falcon.Numerics;
using Lykke.PrivateBlockchain.Definitions;
using MAVN.Service.CrossChainTransfers.Domain.Models;

namespace MAVN.Service.CrossChainTransfers.MappingProfiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<TransferredFromPublicNetworkEventDTO, TransferToInternalEventDto>()
                .ForMember(x => x.InternalAddress, opt => opt.MapFrom(p => p.InternalAccount))
                .ForMember(x => x.Amount, opt => opt.MapFrom(p => Money18.CreateFromAtto(p.Amount)))
                .ForMember(x => x.PublicAddress, opt => opt.MapFrom(p => p.PublicAccount));
            CreateMap<TransferredToPublicNetworkEventDTO, TransferToExternalEventDto>()
                .ForMember(x => x.InternalAddress, opt => opt.MapFrom(p => p.InternalAccount))
                .ForMember(x => x.Amount, opt => opt.MapFrom(p => Money18.CreateFromAtto(p.Amount)))
                .ForMember(x => x.PublicAddress, opt => opt.MapFrom(p => p.PublicAccount));
        }
    }
}
