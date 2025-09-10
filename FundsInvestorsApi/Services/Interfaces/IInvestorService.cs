using FundsInvestorsApi.DTOs;

namespace FundsInvestorsApi.Services
{
    public interface IInvestorService
    {
        Task<IEnumerable<InvestorDto>> GetAllAsync();
        Task<InvestorDto?> GetByIdAsync(Guid id);
        Task<InvestorDto> CreateAsync(InvestorCreateDto dto);
        Task UpdateAsync(InvestorUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}
