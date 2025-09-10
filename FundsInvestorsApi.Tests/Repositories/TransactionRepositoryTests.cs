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
    /// Unit tests for TransactionRepository using in-memory EF Core database.
    /// </summary>
    public class TransactionRepositoryTests
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
        public async Task AddAsync_ShouldAddTransaction()
        {
            // Arrange: create context, repository, and a new transaction
            await using var context = await CreateDbContextAsync();
            var repo = new TransactionRepository(context);
            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                InvestorId = Guid.NewGuid(),
                Type = TransactionType.Subscription,
                Amount = 1000m,
                TransactionDate = DateTime.UtcNow
            };

            // Act: add transaction and save changes
            await repo.AddAsync(transaction);
            await repo.SaveChangesAsync();

            // Assert: transaction exists in database
            var found = await context.Transactions.FindAsync(transaction.TransactionId);
            Assert.NotNull(found);
            Assert.Equal(1000m, found.Amount);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTransactions()
        {
            await using var context = await CreateDbContextAsync();
            var repo = new TransactionRepository(context);

            // Arrange: add multiple transactions
            var transaction1 = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                InvestorId = Guid.NewGuid(),
                Type = TransactionType.Subscription,
                Amount = 500m,
                TransactionDate = DateTime.UtcNow
            };
            var transaction2 = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                InvestorId = Guid.NewGuid(),
                Type = TransactionType.Redemption,
                Amount = 200m,
                TransactionDate = DateTime.UtcNow
            };
            await context.Transactions.AddRangeAsync(transaction1, transaction2);
            await context.SaveChangesAsync();

            // Act: get all transactions
            var result = await repo.GetAllAsync();

            // Assert: all added transactions are returned
            Assert.Contains(result, t => t.Amount == 500m);
            Assert.Contains(result, t => t.Amount == 200m);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectTransaction()
        {
            await using var context = await CreateDbContextAsync();
            var repo = new TransactionRepository(context);

            // Arrange: add a single transaction
            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                InvestorId = Guid.NewGuid(),
                Type = TransactionType.Subscription,
                Amount = 750m,
                TransactionDate = DateTime.UtcNow
            };
            await context.Transactions.AddAsync(transaction);
            await context.SaveChangesAsync();

            // Act: get transaction by ID
            var result = await repo.GetByIdAsync(transaction.TransactionId);

            // Assert: correct transaction returned
            Assert.NotNull(result);
            Assert.Equal(750m, result.Amount);
        }
    }
}
