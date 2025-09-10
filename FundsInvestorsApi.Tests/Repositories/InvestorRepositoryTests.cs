using FundsInvestorsApi.Data;
using FundsInvestorsApi.Models;
using FundsInvestorsApi.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace FundsInvestorsApi.Tests.Repositories
{
    /// <summary>
    /// Unit tests for InvestorRepository using in-memory EF Core database.
    /// </summary>
    public class InvestorRepositoryTests
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
            await context.Database.EnsureCreatedAsync(); // Ensure database is created
            return context;
        }

        [Fact]
        public async Task AddAsync_ShouldAddInvestor()
        {
            // Arrange
            await using var context = await CreateDbContextAsync();
            var repo = new InvestorRepository(context);
            var investor = new Investor
            {
                InvestorId = Guid.NewGuid(),
                FullName = "Test Investor",
                Email = "test@investor.com",
                FundId = Guid.NewGuid()
            };

            // Act: add investor and save changes
            await repo.AddAsync(investor);
            await repo.SaveChangesAsync();

            // Assert: investor exists in database
            var found = await context.Investors.FindAsync(investor.InvestorId);
            Assert.NotNull(found);
            Assert.Equal("Test Investor", found.FullName);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllInvestors()
        {
            await using var context = await CreateDbContextAsync();
            var repo = new InvestorRepository(context);

            // Arrange: add multiple investors
            var investor1 = new Investor { InvestorId = Guid.NewGuid(), FullName = "Investor1", Email = "i1@investor.com", FundId = Guid.NewGuid() };
            var investor2 = new Investor { InvestorId = Guid.NewGuid(), FullName = "Investor2", Email = "i2@investor.com", FundId = Guid.NewGuid() };
            await context.Investors.AddRangeAsync(investor1, investor2);
            await context.SaveChangesAsync();

            // Act: retrieve all investors
            var result = await repo.GetAllAsync();

            // Assert: all added investors are returned
            Assert.Contains(result, i => i.FullName == "Investor1");
            Assert.Contains(result, i => i.FullName == "Investor2");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectInvestor()
        {
            await using var context = await CreateDbContextAsync();
            var repo = new InvestorRepository(context);

            // Arrange: add a single investor
            var investor = new Investor { InvestorId = Guid.NewGuid(), FullName = "InvestorX", Email = "x@investor.com", FundId = Guid.NewGuid() };
            await context.Investors.AddAsync(investor);
            await context.SaveChangesAsync();

            // Act: get investor by ID
            var result = await repo.GetByIdAsync(investor.InvestorId);

            // Assert: correct investor returned
            Assert.NotNull(result);
            Assert.Equal("InvestorX", result.FullName);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateInvestor()
        {
            await using var context = await CreateDbContextAsync();
            var repo = new InvestorRepository(context);

            // Arrange: add investor
            var investor = new Investor { InvestorId = Guid.NewGuid(), FullName = "OldName", Email = "old@investor.com", FundId = Guid.NewGuid() };
            await context.Investors.AddAsync(investor);
            await context.SaveChangesAsync();

            // Act: update investor name
            investor.FullName = "NewName";
            await repo.UpdateAsync(investor);
            await repo.SaveChangesAsync();

            // Assert: name is updated in database
            var updated = await context.Investors.FindAsync(investor.InvestorId);
            Assert.Equal("NewName", updated.FullName);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveInvestor()
        {
            await using var context = await CreateDbContextAsync();
            var repo = new InvestorRepository(context);

            // Arrange: add investor to delete
            var investor = new Investor { InvestorId = Guid.NewGuid(), FullName = "ToDelete", Email = "delete@investor.com", FundId = Guid.NewGuid() };
            await context.Investors.AddAsync(investor);
            await context.SaveChangesAsync();

            // Act: delete investor
            await repo.DeleteAsync(investor.InvestorId);
            await repo.SaveChangesAsync();

            // Assert: investor no longer exists
            var deleted = await context.Investors.FindAsync(investor.InvestorId);
            Assert.Null(deleted);
        }
    }
}
