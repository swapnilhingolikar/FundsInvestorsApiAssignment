using FundsInvestorsApi.Data;
using FundsInvestorsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FundsInvestorsApi.Repositories
{
    /// <summary>
    /// Repository for managing Transaction entities.
    /// </summary>
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Transaction>> GetAllAsync() =>
            await _context.Transactions.ToListAsync();

        public async Task<Transaction?> GetByIdAsync(Guid id) =>
            await _context.Transactions.FindAsync(id);

        public async Task<IEnumerable<Transaction>> GetByInvestorIdAsync(Guid investorId) =>
            await _context.Transactions
                .Where(t => t.InvestorId == investorId)
                .ToListAsync();

        public async Task AddAsync(Transaction transaction) =>
            await _context.Transactions.AddAsync(transaction);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
