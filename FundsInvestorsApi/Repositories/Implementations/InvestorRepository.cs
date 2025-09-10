using FundsInvestorsApi.Data;
using FundsInvestorsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FundsInvestorsApi.Repositories
{
    public class InvestorRepository : IInvestorRepository
    {
        private readonly AppDbContext _context;
        public InvestorRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Investor>> GetAllAsync() => await _context.Investors.ToListAsync();

        public async Task<Investor?> GetByIdAsync(Guid id) => await _context.Investors.FindAsync(id);

        public async Task AddAsync(Investor investor) => await _context.Investors.AddAsync(investor);

        public Task UpdateAsync(Investor investor)
        {
            _context.Investors.Update(investor);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var investor = await _context.Investors.FindAsync(id);
            if (investor != null) _context.Investors.Remove(investor);
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
