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
    /// Unit tests for InvestorService using mocked repository and AutoMapper.
    /// </summary>
    public class InvestorServiceTests
    {
        private readonly Mock<IInvestorRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly InvestorService _service;

        public InvestorServiceTests()
        {
            _repoMock = new Mock<IInvestorRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new InvestorService(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedInvestorDtos()
        {
            // Arrange: mock repository and mapper
            var investors = new List<Investor>
            {
                new Investor { InvestorId = Guid.NewGuid(), FullName = "John Doe", Email = "john@example.com", FundId = Guid.NewGuid() }
            };
            var investorDtos = new List<InvestorDto>
            {
                new InvestorDto { InvestorId = investors[0].InvestorId, FullName = investors[0].FullName, Email = investors[0].Email, FundId = investors[0].FundId }
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(investors);
            _mapperMock.Setup(m => m.Map<IEnumerable<InvestorDto>>(investors)).Returns(investorDtos);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<InvestorDto>>(investors), Times.Once);
            Assert.Equal(investorDtos, result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMappedInvestorDto()
        {
            // Arrange: prepare mock for single investor
            var id = Guid.NewGuid();
            var investor = new Investor { InvestorId = id, FullName = "Jane Smith", Email = "jane@example.com", FundId = Guid.NewGuid() };
            var investorDto = new InvestorDto { InvestorId = id, FullName = investor.FullName, Email = investor.Email, FundId = investor.FundId };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(investor);
            _mapperMock.Setup(m => m.Map<InvestorDto?>(investor)).Returns(investorDto);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            _mapperMock.Verify(m => m.Map<InvestorDto?>(investor), Times.Once);
            Assert.Equal(investorDto, result);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            // Arrange: repository returns null
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Investor)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert: service returns null
            Assert.Null(result);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddAndReturnInvestorDto()
        {
            // Arrange: prepare create DTO and mapped entity
            var createDto = new InvestorCreateDto { FullName = "Alice Brown", Email = "alice@example.com", FundId = Guid.NewGuid() };
            var investor = new Investor { InvestorId = Guid.NewGuid(), FullName = createDto.FullName, Email = createDto.Email, FundId = createDto.FundId };
            var investorDto = new InvestorDto { InvestorId = investor.InvestorId, FullName = investor.FullName, Email = investor.Email, FundId = investor.FundId };

            _mapperMock.Setup(m => m.Map<Investor>(createDto)).Returns(investor);
            _repoMock.Setup(r => r.AddAsync(investor)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<InvestorDto>(investor)).Returns(investorDto);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            _mapperMock.Verify(m => m.Map<Investor>(createDto), Times.Once);
            _repoMock.Verify(r => r.AddAsync(investor), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<InvestorDto>(investor), Times.Once);
            Assert.Equal(investorDto, result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateInvestor()
        {
            // Arrange: prepare update DTO and mapped entity
            var updateDto = new InvestorUpdateDto { InvestorId = Guid.NewGuid(), FullName = "Bob Green", Email = "bob@example.com", FundId = Guid.NewGuid() };
            var investor = new Investor { InvestorId = updateDto.InvestorId, FullName = updateDto.FullName, Email = updateDto.Email, FundId = updateDto.FundId };

            _mapperMock.Setup(m => m.Map<Investor>(updateDto)).Returns(investor);
            _repoMock.Setup(r => r.UpdateAsync(investor)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(updateDto);

            // Assert: verify calls
            _mapperMock.Verify(m => m.Map<Investor>(updateDto), Times.Once);
            _repoMock.Verify(r => r.UpdateAsync(investor), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteInvestor()
        {
            // Arrange: prepare mock repository
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(id);

            // Assert: verify repository methods called
            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
