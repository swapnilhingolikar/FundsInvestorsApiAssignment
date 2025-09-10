using FundsInvestorsApi.Data;
using FundsInvestorsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FundsInvestorsApi.Repositories
{
    public class FundRepository : IFundRepository
    {
        private readonly AppDbContext _context;
        public FundRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Fund>> GetAllAsync() => await _context.Funds.ToListAsync();

        public async Task<Fund?> GetByIdAsync(Guid id) => await _context.Funds.FindAsync(id);

        public async Task AddAsync(Fund fund) => await _context.Funds.AddAsync(fund);

        public Task UpdateAsync(Fund fund)
        {
            _context.Funds.Update(fund);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var fund = await _context.Funds.FindAsync(id);
            if (fund != null) _context.Funds.Remove(fund);
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
