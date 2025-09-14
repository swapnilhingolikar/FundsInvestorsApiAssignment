using FundsInvestorsApi.Data;
using FundsInvestorsApi.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FundsInvestorsApi.Repositories
{
    /// <summary>
    /// Repository for managing Transaction entities.
    /// Handles all database operations for Transactions.
    /// </summary>
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            try
            {
                return await _context.Transactions.ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving all transactions");
                throw;
            }
        }

        public async Task<Transaction?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.Transactions.FindAsync(id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving transaction with ID {TransactionId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Transaction>> GetByInvestorIdAsync(Guid investorId)
        {
            try
            {
                return await _context.Transactions
                    .Where(t => t.InvestorId == investorId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving transactions for investor ID {InvestorId}", investorId);
                throw;
            }
        }

        public async Task AddAsync(Transaction transaction)
        {
            try
            {
                await _context.Transactions.AddAsync(transaction);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding transaction {@Transaction}", transaction);
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error saving changes to the database");
                throw;
            }
        }
    }
}
