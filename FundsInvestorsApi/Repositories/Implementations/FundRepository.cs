using FundsInvestorsApi.Data;
using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FundsInvestorsApi.Repositories
{
    /// <summary>
    /// Repository for managing Fund entities.
    /// Handles all database operations for Funds.
    /// </summary>
    public class FundRepository : IFundRepository
    {
        private readonly AppDbContext _context;

        public FundRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Fund>> GetAllAsync()
        {
            try
            {
                return await _context.Funds.ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving all funds");
                throw;
            }
        }

        public async Task<Fund?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.Funds.FindAsync(id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving fund with ID {FundId}", id);
                throw;
            }
        }

        public async Task AddAsync(Fund fund)
        {
            try
            {
                await _context.Funds.AddAsync(fund);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding fund {@Fund}", fund);
                throw;
            }
        }

        public Task UpdateAsync(Fund fund)
        {
            try
            {
                _context.Funds.Update(fund);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating fund {@Fund}", fund);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var fund = await _context.Funds.FindAsync(id);
                if (fund != null)
                    _context.Funds.Remove(fund);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting fund with ID {FundId}", id);
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

        public async Task<IEnumerable<FundTransactionSummaryDto>> GetTransactionSummaryAsync()
        {
            // Query all funds from the database
            return await _context.Funds
                .Select(f => new FundTransactionSummaryDto
                {
                    // Assign fund ID and name
                    FundId = f.FundId,
                    FundName = f.Name,

                    // Calculate total subscribed amount for the fund
                    // Handle nulls using ?? and avoid exceptions if Investors or Transactions are null
                    TotalSubscribed = (f.Investors ?? Enumerable.Empty<Investor>())        // If no investors, use empty list
                                        .SelectMany(i => i.Transactions ?? Enumerable.Empty<Transaction>()) // Flatten all transactions
                                        .Where(t => t.Type == FundsInvestorsApi.Models.TransactionType.Subscription) // Only Subscription transactions
                                        .Sum(t => t.Amount), // Sum their amounts

                    // Calculate total redeemed amount for the fund
                    TotalRedeemed = (f.Investors ?? Enumerable.Empty<Investor>())
                                        .SelectMany(i => i.Transactions ?? Enumerable.Empty<Transaction>())
                                        .Where(t => t.Type == FundsInvestorsApi.Models.TransactionType.Redemption) // Only Redemption transactions
                                        .Sum(t => t.Amount)
                })
                .ToListAsync(); // Execute query asynchronously and return list
        }


    }
}
