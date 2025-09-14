using FundsInvestorsApi.DTOs;

namespace FundsInvestorsApi.Services
{
    public interface IFundService
    {
        Task<IEnumerable<FundDto>> GetAllAsync();
        Task<FundDto?> GetByIdAsync(Guid id);
        Task<FundDto> CreateAsync(FundCreateDto dto);
        Task UpdateAsync(FundUpdateDto dto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<FundTransactionSummaryDto>> GetTransactionSummaryAsync();
    }
}
