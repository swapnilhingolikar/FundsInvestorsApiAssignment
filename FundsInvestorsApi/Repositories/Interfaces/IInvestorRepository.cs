using FundsInvestorsApi.Models;

namespace FundsInvestorsApi.Repositories
{
    public interface IInvestorRepository
    {
        Task<IEnumerable<Investor>> GetAllAsync();
        Task<Investor?> GetByIdAsync(Guid id);
        Task AddAsync(Investor investor);
        Task UpdateAsync(Investor investor);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}