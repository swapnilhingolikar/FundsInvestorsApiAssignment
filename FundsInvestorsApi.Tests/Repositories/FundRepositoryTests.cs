using FundsInvestorsApi.Data;
using FundsInvestorsApi.Models;
using FundsInvestorsApi.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace FundsInvestorsApi.Tests.Repositories
{
    public class FundRepositoryTests
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
        public async Task AddAsync_ShouldAddFund()
        {
            var context = await GetDbContextAsync();
            var repo = new FundRepository(context);
            var fund = new Fund { FundId = Guid.NewGuid(), Name = "Test Fund", Currency = "USD", LaunchDate = DateTime.UtcNow };

            await repo.AddAsync(fund);
            await context.SaveChangesAsync();

            var found = await context.Funds.FindAsync(fund.FundId);
            Assert.NotNull(found);
            Assert.Equal("Test Fund", found.Name);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllFunds()
        {
            var context = await GetDbContextAsync();
            var repo = new FundRepository(context);
            var fund1 = new Fund { FundId = Guid.NewGuid(), Name = "Fund1", Currency = "USD", LaunchDate = DateTime.UtcNow };
            var fund2 = new Fund { FundId = Guid.NewGuid(), Name = "Fund2", Currency = "EUR", LaunchDate = DateTime.UtcNow };
            await context.Funds.AddRangeAsync(fund1, fund2);
            await context.SaveChangesAsync();

            var result = await repo.GetAllAsync();
            Assert.Contains(result, f => f.Name == "Fund1");
            Assert.Contains(result, f => f.Name == "Fund2");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectFund()
        {
            var context = await GetDbContextAsync();
            var repo = new FundRepository(context);
            var fund = new Fund { FundId = Guid.NewGuid(), Name = "FundX", Currency = "GBP", LaunchDate = DateTime.UtcNow };
            await context.Funds.AddAsync(fund);
            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(fund.FundId);
            Assert.NotNull(result);
            Assert.Equal("FundX", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateFund()
        {
            var context = await GetDbContextAsync();
            var repo = new FundRepository(context);
            var fund = new Fund { FundId = Guid.NewGuid(), Name = "OldName", Currency = "USD", LaunchDate = DateTime.UtcNow };
            await context.Funds.AddAsync(fund);
            await context.SaveChangesAsync();

            fund.Name = "NewName";
            await repo.UpdateAsync(fund);
            await context.SaveChangesAsync();

            var updated = await context.Funds.FindAsync(fund.FundId);
            Assert.Equal("NewName", updated.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveFund()
        {
            var context = await GetDbContextAsync();
            var repo = new FundRepository(context);
            var fund = new Fund { FundId = Guid.NewGuid(), Name = "ToDelete", Currency = "USD", LaunchDate = DateTime.UtcNow };
            await context.Funds.AddAsync(fund);
            await context.SaveChangesAsync();

            await repo.DeleteAsync(fund.FundId);
            await context.SaveChangesAsync();

            var deleted = await context.Funds.FindAsync(fund.FundId);
            Assert.Null(deleted);
        }
    }
}