using FundsInvestorsApi.Data;
using FundsInvestorsApi.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FundsInvestorsApi.Repositories
{
    /// <summary>
    /// Repository for managing Investor entities.
    /// Handles all database operations for Investors.
    /// </summary>
    public class InvestorRepository : IInvestorRepository
    {
        private readonly AppDbContext _context;

        public InvestorRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Investor>> GetAllAsync()
        {
            try
            {
                return await _context.Investors.ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving all investors");
                throw;
            }
        }

        public async Task<Investor?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.Investors.FindAsync(id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving investor with ID {InvestorId}", id);
                throw;
            }
        }

        public async Task AddAsync(Investor investor)
        {
            try
            {
                await _context.Investors.AddAsync(investor);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding investor {@Investor}", investor);
                throw;
            }
        }

        public Task UpdateAsync(Investor investor)
        {
            try
            {
                _context.Investors.Update(investor);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating investor {@Investor}", investor);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var investor = await _context.Investors.FindAsync(id);
                if (investor != null)
                    _context.Investors.Remove(investor);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting investor with ID {InvestorId}", id);
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
