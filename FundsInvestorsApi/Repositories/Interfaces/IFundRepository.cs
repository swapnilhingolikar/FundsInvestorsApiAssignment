using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Models;

namespace FundsInvestorsApi.Repositories
{
    public interface IFundRepository
    {
        Task<IEnumerable<Fund>> GetAllAsync();
        Task<Fund?> GetByIdAsync(Guid id);
        Task AddAsync(Fund fund);
        Task UpdateAsync(Fund fund);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
        Task<IEnumerable<FundTransactionSummaryDto>> GetTransactionSummaryAsync();
    }
}