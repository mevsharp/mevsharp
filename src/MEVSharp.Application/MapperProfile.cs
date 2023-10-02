using AutoMapper;
using MEVSharp.Features.Http.Clients.Dtos;

namespace MEVSharp.Application
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<RegisterValidatorRequestDTO, RegisterValidatorRequestInnerRequest>()
                .ForMember(src => src.Message, opts => opts.MapFrom(dest => dest.Message))
                .ForMember(src => src.Signature, opts => opts.MapFrom(dest => dest.Signature));

            CreateMap<RegisterValidatorRequestMessageDTO, RegisterValidatorRequestMessageRequest>()
                .ForMember(src => src.PubKey, opts => opts.MapFrom(dest => dest.PubKey))
                .ForMember(src => src.FeeRecipient, opts => opts.MapFrom(dest => dest.FeeRecipient))
                .ForMember(src => src.GasLimit, opts => opts.MapFrom(dest => dest.GasLimit))
                .ForMember(src => src.Timestamp, opts => opts.MapFrom(dest => dest.TimeStamp));

            //CreateMap<BlindedBlockRequest, SubmitBlindedBlockRequest>();
        }
    }
}
