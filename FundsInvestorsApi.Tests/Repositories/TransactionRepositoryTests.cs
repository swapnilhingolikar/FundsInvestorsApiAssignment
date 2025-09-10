using FundsInvestorsApi.Data;
using FundsInvestorsApi.Models;
using FundsInvestorsApi.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace FundsInvestorsApi.Tests.Repositories
{
    public class TransactionRepositoryTests
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
        public async Task AddAsync_ShouldAddTransaction()
        {
            var context = await GetDbContextAsync();
            var repo = new TransactionRepository(context);
            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                InvestorId = Guid.NewGuid(),
                Type = TransactionType.Subscription,
                Amount = 1000m,
                TransactionDate = DateTime.UtcNow
            };

            await repo.AddAsync(transaction);
            await context.SaveChangesAsync();

            var found = await context.Transactions.FindAsync(transaction.TransactionId);
            Assert.NotNull(found);
            Assert.Equal(1000m, found.Amount);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTransactions()
        {
            var context = await GetDbContextAsync();
            var repo = new TransactionRepository(context);
            var transaction1 = new Transaction { TransactionId = Guid.NewGuid(), InvestorId = Guid.NewGuid(), Type = TransactionType.Subscription, Amount = 500m, TransactionDate = DateTime.UtcNow };
            var transaction2 = new Transaction { TransactionId = Guid.NewGuid(), InvestorId = Guid.NewGuid(), Type = TransactionType.Redemption, Amount = 200m, TransactionDate = DateTime.UtcNow };
            await context.Transactions.AddRangeAsync(transaction1, transaction2);
            await context.SaveChangesAsync();

            var result = await repo.GetAllAsync();
            Assert.Contains(result, t => t.Amount == 500m);
            Assert.Contains(result, t => t.Amount == 200m);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectTransaction()
        {
            var context = await GetDbContextAsync();
            var repo = new TransactionRepository(context);
            var transaction = new Transaction { TransactionId = Guid.NewGuid(), InvestorId = Guid.NewGuid(), Type = TransactionType.Subscription, Amount = 750m, TransactionDate = DateTime.UtcNow };
            await context.Transactions.AddAsync(transaction);
            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(transaction.TransactionId);
            Assert.NotNull(result);
            Assert.Equal(750m, result.Amount);
        }
    }
}