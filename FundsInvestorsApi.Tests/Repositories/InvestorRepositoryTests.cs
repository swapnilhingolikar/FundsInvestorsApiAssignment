using FundsInvestorsApi.Data;
using FundsInvestorsApi.Models;
using FundsInvestorsApi.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace FundsInvestorsApi.Tests.Repositories
{
    public class InvestorRepositoryTests
    {
        private async Task<AppDbContext> GetDbContextAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new AppDbContext(options);
            await context.Database.EnsureCreatedAsync();
            return context;
        }

        [Fact]
        public async Task AddAsync_ShouldAddInvestor()
        {
            var context = await GetDbContextAsync();
            var repo = new InvestorRepository(context);
            var investor = new Investor
            {
                InvestorId = Guid.NewGuid(),
                FullName = "Test Investor",
                Email = "test@investor.com",
                FundId = Guid.NewGuid()
            };

            await repo.AddAsync(investor);
            await context.SaveChangesAsync();

            var found = await context.Investors.FindAsync(investor.InvestorId);
            Assert.NotNull(found);
            Assert.Equal("Test Investor", found.FullName);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllInvestors()
        {
            var context = await GetDbContextAsync();
            var repo = new InvestorRepository(context);
            var investor1 = new Investor { InvestorId = Guid.NewGuid(), FullName = "Investor1", Email = "i1@investor.com", FundId = Guid.NewGuid() };
            var investor2 = new Investor { InvestorId = Guid.NewGuid(), FullName = "Investor2", Email = "i2@investor.com", FundId = Guid.NewGuid() };
            await context.Investors.AddRangeAsync(investor1, investor2);
            await context.SaveChangesAsync();

            var result = await repo.GetAllAsync();
            Assert.Contains(result, i => i.FullName == "Investor1");
            Assert.Contains(result, i => i.FullName == "Investor2");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectInvestor()
        {
            var context = await GetDbContextAsync();
            var repo = new InvestorRepository(context);
            var investor = new Investor { InvestorId = Guid.NewGuid(), FullName = "InvestorX", Email = "x@investor.com", FundId = Guid.NewGuid() };
            await context.Investors.AddAsync(investor);
            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(investor.InvestorId);
            Assert.NotNull(result);
            Assert.Equal("InvestorX", result.FullName);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateInvestor()
        {
            var context = await GetDbContextAsync();
            var repo = new InvestorRepository(context);
            var investor = new Investor { InvestorId = Guid.NewGuid(), FullName = "OldName", Email = "old@investor.com", FundId = Guid.NewGuid() };
            await context.Investors.AddAsync(investor);
            await context.SaveChangesAsync();

            investor.FullName = "NewName";
            await repo.UpdateAsync(investor);
            await context.SaveChangesAsync();

            var updated = await context.Investors.FindAsync(investor.InvestorId);
            Assert.Equal("NewName", updated.FullName);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveInvestor()
        {
            var context = await GetDbContextAsync();
            var repo = new InvestorRepository(context);
            var investor = new Investor { InvestorId = Guid.NewGuid(), FullName = "ToDelete", Email = "delete@investor.com", FundId = Guid.NewGuid() };
            await context.Investors.AddAsync(investor);
            await context.SaveChangesAsync();

            await repo.DeleteAsync(investor.InvestorId);
            await context.SaveChangesAsync();

            var deleted = await context.Investors.FindAsync(investor.InvestorId);
            Assert.Null(deleted);
        }
    }
}