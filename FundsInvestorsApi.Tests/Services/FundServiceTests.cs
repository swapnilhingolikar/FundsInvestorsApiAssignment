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

namespace FundsInvestorsApi.Tests.Services
{
    /// <summary>
    /// Unit tests for FundService using mocked repository and AutoMapper.
    /// </summary>
    public class FundServiceTests
    {
        private readonly Mock<IFundRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly FundService _service;

        public FundServiceTests()
        {
            _repoMock = new Mock<IFundRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new FundService(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedFundDtos()
        {
            // Arrange: mock repository and mapper
            var funds = new List<Fund>
            {
                new Fund { FundId = Guid.NewGuid(), Name = "Fund A", Currency = "USD", LaunchDate = DateTime.UtcNow }
            };
            var fundDtos = new List<FundDto>
            {
                new FundDto { FundId = funds[0].FundId, Name = funds[0].Name, Currency = funds[0].Currency, LaunchDate = funds[0].LaunchDate }
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(funds);
            _mapperMock.Setup(m => m.Map<IEnumerable<FundDto>>(funds)).Returns(fundDtos);

            // Act: call service method
            var result = await _service.GetAllAsync();

            // Assert: returned DTOs match expected
            Assert.Equal(fundDtos, result);
            _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<FundDto>>(funds), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMappedFundDto()
        {
            // Arrange: mock repository and mapper
            var id = Guid.NewGuid();
            var fund = new Fund { FundId = id, Name = "Fund B", Currency = "EUR", LaunchDate = DateTime.UtcNow };
            var fundDto = new FundDto { FundId = id, Name = fund.Name, Currency = fund.Currency, LaunchDate = fund.LaunchDate };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(fund);
            _mapperMock.Setup(m => m.Map<FundDto?>(fund)).Returns(fundDto);

            // Act: call service method
            var result = await _service.GetByIdAsync(id);

            // Assert: returned DTO matches expected
            Assert.Equal(fundDto, result);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            _mapperMock.Verify(m => m.Map<FundDto?>(fund), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            // Arrange: repository returns null
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Fund)null);

            // Act: call service method
            var result = await _service.GetByIdAsync(id);

            // Assert: result is null
            Assert.Null(result);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddFundAndReturnDto()
        {
            // Arrange: prepare create DTO and mapped fund
            var createDto = new FundCreateDto { Name = "Test Fund", Currency = "USD", LaunchDate = DateTime.UtcNow };
            var fund = new Fund { FundId = Guid.NewGuid(), Name = createDto.Name, Currency = createDto.Currency, LaunchDate = createDto.LaunchDate };
            var returnedDto = new FundDto { FundId = fund.FundId, Name = fund.Name, Currency = fund.Currency, LaunchDate = fund.LaunchDate };

            _mapperMock.Setup(m => m.Map<Fund>(createDto)).Returns(fund);
            _repoMock.Setup(r => r.AddAsync(fund)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<FundDto>(fund)).Returns(returnedDto);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.Equal(returnedDto.Name, result.Name);
            Assert.Equal(returnedDto.Currency, result.Currency);
            Assert.Equal(returnedDto.LaunchDate, result.LaunchDate);
            _mapperMock.Verify(m => m.Map<Fund>(createDto), Times.Once);
            _repoMock.Verify(r => r.AddAsync(fund), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<FundDto>(fund), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_NullInput_ThrowsArgumentNullException()
        {
            // Act & Assert: passing null should throw
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(null));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateFund()
        {
            // Arrange: prepare update DTO and mapped fund
            var updateDto = new FundUpdateDto { FundId = Guid.NewGuid(), Name = "Updated Fund", Currency = "GBP", LaunchDate = DateTime.UtcNow };
            var fund = new Fund { FundId = updateDto.FundId, Name = updateDto.Name, Currency = updateDto.Currency, LaunchDate = updateDto.LaunchDate };

            _mapperMock.Setup(m => m.Map<Fund>(updateDto)).Returns(fund);
            _repoMock.Setup(r => r.UpdateAsync(fund)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(updateDto);

            // Assert: verify repository and mapper calls
            _mapperMock.Verify(m => m.Map<Fund>(updateDto), Times.Once);
            _repoMock.Verify(r => r.UpdateAsync(fund), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteFund()
        {
            // Arrange: prepare fund ID
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(id);

            // Assert: repository methods called
            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
