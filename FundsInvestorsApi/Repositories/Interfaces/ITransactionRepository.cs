using FundsInvestorsApi.Models;

namespace FundsInvestorsApi.Repositories
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<Transaction?> GetByIdAsync(Guid id);
        Task<IEnumerable<Transaction>> GetByInvestorIdAsync(Guid investorId);
        Task AddAsync(Transaction transaction);
        Task SaveChangesAsync();
    }
}