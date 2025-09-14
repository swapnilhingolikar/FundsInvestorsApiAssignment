using AutoMapper;
using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Models;
using FundsInvestorsApi.Repositories;
using FundsInvestorsApi.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;

namespace FundsInvestorsApi.Tests.Services
{
    /// <summary>
    /// Unit tests for TransactionService using mocked repository and AutoMapper.
    /// </summary>
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<TransactionService>> _loggerMock;
        private readonly TransactionService _service;

        public TransactionServiceTests()
        {
            _repoMock = new Mock<ITransactionRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<TransactionService>>();
            _service = new TransactionService(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedTransactionDtos()
        {
            // Arrange: mock repository returning a list of transactions and mapper returning DTOs
            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    TransactionId = Guid.NewGuid(),
                    InvestorId = Guid.NewGuid(),
                    Type = Models.TransactionType.Subscription,
                    Amount = 1000m,
                    TransactionDate = DateTime.UtcNow
                }
            };
            var transactionDtos = new List<TransactionDto>
            {
                new TransactionDto
                {
                    TransactionId = transactions[0].TransactionId,
                    InvestorId = transactions[0].InvestorId,
                    Type = DTOs.TransactionType.Subscription,
                    Amount = 1000m,
                    TransactionDate = transactions[0].TransactionDate
                }
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(transactions);
            _mapperMock.Setup(m => m.Map<IEnumerable<TransactionDto>>(transactions)).Returns(transactionDtos);

            // Act
            var result = await _service.GetAllAsync();

            // Assert: verify repository call, mapper call, and returned data
            Assert.Equal(transactionDtos, result);
            _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<TransactionDto>>(transactions), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMappedTransactionDto()
        {
            // Arrange: mock single transaction and mapped DTO
            var id = Guid.NewGuid();
            var transaction = new Transaction
            {
                TransactionId = id,
                InvestorId = Guid.NewGuid(),
                Type = Models.TransactionType.Subscription,
                Amount = 500m,
                TransactionDate = DateTime.UtcNow
            };
            var transactionDto = new TransactionDto
            {
                TransactionId = id,
                InvestorId = transaction.InvestorId,
                Type = DTOs.TransactionType.Subscription,
                Amount = 500m,
                TransactionDate = transaction.TransactionDate
            };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(transaction);
            _mapperMock.Setup(m => m.Map<TransactionDto>(transaction)).Returns(transactionDto);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.Equal(transactionDto, result);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            _mapperMock.Verify(m => m.Map<TransactionDto>(transaction), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            // Arrange: repository returns null for unknown transaction
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Transaction)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert: service should return null
            Assert.Null(result);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddAndReturnTransactionDto()
        {
            // Arrange: prepare create DTO and corresponding mapped entity and DTO
            var createDto = new TransactionCreateDto
            {
                InvestorId = Guid.NewGuid(),
                Type = DTOs.TransactionType.Subscription,
                Amount = 1000m,
                TransactionDate = DateTime.UtcNow
            };
            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                InvestorId = createDto.InvestorId,
                Type = Models.TransactionType.Subscription,
                Amount = createDto.Amount,
                TransactionDate = createDto.TransactionDate
            };
            var transactionDto = new TransactionDto
            {
                TransactionId = transaction.TransactionId,
                InvestorId = transaction.InvestorId,
                Type = DTOs.TransactionType.Subscription,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate
            };

            _mapperMock.Setup(m => m.Map<Transaction>(createDto)).Returns(transaction);
            _repoMock.Setup(r => r.AddAsync(transaction)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<TransactionDto>(transaction)).Returns(transactionDto);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert: verify mapping, repository calls, and returned DTO
            Assert.Equal(transactionDto, result);
            _mapperMock.Verify(m => m.Map<Transaction>(createDto), Times.Once);
            _repoMock.Verify(r => r.AddAsync(transaction), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<TransactionDto>(transaction), Times.Once);
        }
    }
}