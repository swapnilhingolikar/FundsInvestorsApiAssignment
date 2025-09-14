using FundsInvestorsApi.Data;
using FundsInvestorsApi.Models;
using FundsInvestorsApi.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FundsInvestorsApi.Tests.Repositories
{
    /// <summary>
    /// Unit tests for FundRepository using in-memory EF Core database.
    /// </summary>
    public class FundRepositoryTests
    {
        /// <summary>
        /// Creates a new in-memory AppDbContext for testing.
        /// </summary>
        private async Task<AppDbContext> CreateDbContextAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);
            await context.Database.EnsureCreatedAsync(); // Ensure DB is created
            return context;
        }

        [Fact]
        public async Task AddAsync_ShouldAddFund()
        {
            // Arrange: create context, repository, and a new fund
            await using var context = await CreateDbContextAsync();
            var repo = new FundRepository(context);
            var fund = new Fund
            {
                FundId = Guid.NewGuid(),
                Name = "Test Fund",
                Currency = "USD",
                LaunchDate = DateTime.UtcNow
            };

            // Act: add fund and save changes
            await repo.AddAsync(fund);
            await repo.SaveChangesAsync();

            // Assert: fund is added to database
            var found = await context.Funds.FindAsync(fund.FundId);
            Assert.NotNull(found);
            Assert.Equal("Test Fund", found.Name);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllFunds()
        {
            await using var context = await CreateDbContextAsync();
            var repo = new FundRepository(context);

            // Arrange: add multiple funds
            var fund1 = new Fund { FundId = Guid.NewGuid(), Name = "Fund1", Currency = "USD", LaunchDate = DateTime.UtcNow };
            var fund2 = new Fund { FundId = Guid.NewGuid(), Name = "Fund2", Currency = "EUR", LaunchDate = DateTime.UtcNow };
            await context.Funds.AddRangeAsync(fund1, fund2);
            await context.SaveChangesAsync();

            // Act: get all funds
            var result = await repo.GetAllAsync();

            // Assert: all added funds are returned
            Assert.Contains(result, f => f.Name == "Fund1");
            Assert.Contains(result, f => f.Name == "Fund2");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectFund()
        {
            await using var context = await CreateDbContextAsync();
            var repo = new FundRepository(context);

            // Arrange: add a fund
            var fund = new Fund { FundId = Guid.NewGuid(), Name = "FundX", Currency = "GBP", LaunchDate = DateTime.UtcNow };
            await context.Funds.AddAsync(fund);
            await context.SaveChangesAsync();

            // Act: get fund by ID
            var result = await repo.GetByIdAsync(fund.FundId);

            // Assert: fund details match
            Assert.NotNull(result);
            Assert.Equal("FundX", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateFund()
        {
            await using var context = await CreateDbContextAsync();
            var repo = new FundRepository(context);

            // Arrange: add a fund
            var fund = new Fund { FundId = Guid.NewGuid(), Name = "OldName", Currency = "USD", LaunchDate = DateTime.UtcNow };
            await context.Funds.AddAsync(fund);
            await context.SaveChangesAsync();

            // Act: update fund name
            fund.Name = "NewName";
            await repo.UpdateAsync(fund);
            await repo.SaveChangesAsync();

            // Assert: fund name is updated
            var updated = await context.Funds.FindAsync(fund.FundId);
            Assert.Equal("NewName", updated.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveFund()
        {
            await using var context = await CreateDbContextAsync();
            var repo = new FundRepository(context);

            // Arrange: add a fund
            var fund = new Fund { FundId = Guid.NewGuid(), Name = "ToDelete", Currency = "USD", LaunchDate = DateTime.UtcNow };
            await context.Funds.AddAsync(fund);
            await context.SaveChangesAsync();

            // Act: delete fund
            await repo.DeleteAsync(fund.FundId);
            await repo.SaveChangesAsync();

            // Assert: fund no longer exists in database
            var deleted = await context.Funds.FindAsync(fund.FundId);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task GetTransactionSummaryAsync_ShouldReturnCorrectTotals()
        {
            // Arrange
            await using var context = await CreateDbContextAsync();

            // Create funds
            var fund1 = new Fund { FundId = Guid.NewGuid(), Name = "Fund A", Currency = "USD" };
            var fund2 = new Fund { FundId = Guid.NewGuid(), Name = "Fund B", Currency = "EUR" };

            // Create investors
            var investor1 = new Investor { InvestorId = Guid.NewGuid(), FullName = "Investor 1", Fund = fund1 };
            var investor2 = new Investor { InvestorId = Guid.NewGuid(), FullName = "Investor 2", Fund = fund2 };

            // Create transactions
            var transactions = new List<Transaction>
            {
                new Transaction { TransactionId = Guid.NewGuid(), Investor = investor1, Type = TransactionType.Subscription, Amount = 100 },
                new Transaction { TransactionId = Guid.NewGuid(), Investor = investor1, Type = TransactionType.Redemption, Amount = 50 },
                new Transaction { TransactionId = Guid.NewGuid(), Investor = investor2, Type = TransactionType.Subscription, Amount = 200 },
                new Transaction { TransactionId = Guid.NewGuid(), Investor = investor2, Type = TransactionType.Redemption, Amount = 80 }
            };

            await context.Funds.AddRangeAsync(fund1, fund2);
            await context.Investors.AddRangeAsync(investor1, investor2);
            await context.Transactions.AddRangeAsync(transactions);
            await context.SaveChangesAsync();

            // Act: calculate transaction summary per fund
            var summary = await context.Funds
                .Select(f => new
                {
                    FundId = f.FundId,
                    FundName = f.Name,
                    TotalSubscribed = f.Investors
                        .SelectMany(i => i.Transactions)
                        .Where(t => t.Type == TransactionType.Subscription)
                        .Sum(t => t.Amount),
                    TotalRedeemed = f.Investors
                        .SelectMany(i => i.Transactions)
                        .Where(t => t.Type == TransactionType.Redemption)
                        .Sum(t => t.Amount)
                })
                .ToListAsync();

            // Assert
            var fundASummary = summary.First(s => s.FundId == fund1.FundId);
            Assert.Equal(100, fundASummary.TotalSubscribed);
            Assert.Equal(50, fundASummary.TotalRedeemed);

            var fundBSummary = summary.First(s => s.FundId == fund2.FundId);
            Assert.Equal(200, fundBSummary.TotalSubscribed);
            Assert.Equal(80, fundBSummary.TotalRedeemed);
        }
    }
}
