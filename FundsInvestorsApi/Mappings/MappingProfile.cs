using AutoMapper;
using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Models;

namespace FundsInvestorsApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Fund
            CreateMap<Fund, FundDto>();
            CreateMap<FundCreateDto, Fund>()
                .ForMember(dest => dest.FundId, opt => opt.MapFrom(_ => Guid.NewGuid())); // Generate new GUID
            CreateMap<FundUpdateDto, Fund>();

            // Investor
            CreateMap<Investor, InvestorDto>();
            CreateMap<InvestorCreateDto, Investor>()
                .ForMember(dest => dest.InvestorId, opt => opt.MapFrom(_ => Guid.NewGuid())); // Generate new GUID
            CreateMap<InvestorUpdateDto, Investor>();

            // Transaction
            CreateMap<Transaction, TransactionDto>();
            CreateMap<TransactionCreateDto, Transaction>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate));
            CreateMap<TransactionUpdateDto, Transaction>();
        }
    }
}
