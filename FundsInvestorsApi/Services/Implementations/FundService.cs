using AutoMapper;
using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Models;
using FundsInvestorsApi.Repositories;
using Microsoft.Extensions.Logging;

namespace FundsInvestorsApi.Services
{
    /// <summary>
    /// Service class responsible for handling fund-related business logic.
    /// Acts as a bridge between Controllers and Repository layer.
    /// Uses AutoMapper to map between DTOs and entity models.
    /// </summary>
    public class FundService : IFundService
    {
        private readonly IFundRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<FundService> _logger;

        /// <summary>
        /// Constructor with dependency injection for repository, AutoMapper, and logger.
        /// </summary>
        public FundService(IFundRepository repo, IMapper mapper, ILogger<FundService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<FundDto>> GetAllAsync()
        {
            try
            {
                var entities = await _repo.GetAllAsync();
                _logger.LogInformation("Retrieved {Count} funds", entities.Count());
                return _mapper.Map<IEnumerable<FundDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all funds");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<FundDto?> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Fund with ID {FundId} not found", id);
                    return null;
                }

                _logger.LogInformation("Retrieved fund {FundId}", id);
                return _mapper.Map<FundDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving fund with ID {FundId}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<FundDto> CreateAsync(FundCreateDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Attempted to create a fund with null DTO");
                throw new ArgumentNullException(nameof(dto));
            }

            try
            {
                var entity = _mapper.Map<Fund>(dto);
                await _repo.AddAsync(entity);
                await _repo.SaveChangesAsync();

                _logger.LogInformation("Created new fund {FundId} with name {Name}", entity.FundId, entity.Name);
                return _mapper.Map<FundDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating fund {@FundDto}", dto);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(FundUpdateDto dto)
        {
            try
            {
                var entity = _mapper.Map<Fund>(dto);
                await _repo.UpdateAsync(entity);
                await _repo.SaveChangesAsync();

                _logger.LogInformation("Updated fund {FundId}", entity.FundId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating fund {@FundUpdateDto}", dto);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(Guid id)
        {
            try
            {
                await _repo.DeleteAsync(id);
                await _repo.SaveChangesAsync();

                _logger.LogInformation("Deleted fund {FundId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting fund {FundId}", id);
                throw;
            }
        }
    }
}
