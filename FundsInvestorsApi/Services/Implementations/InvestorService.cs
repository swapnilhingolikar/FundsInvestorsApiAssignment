using AutoMapper;
using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Models;
using FundsInvestorsApi.Repositories;
using Microsoft.Extensions.Logging;

namespace FundsInvestorsApi.Services
{
    /// <summary>
    /// Service class responsible for handling investor-related business logic.
    /// Acts as a bridge between the Controller and Repository layers.
    /// Uses AutoMapper to map between DTOs and entity models.
    /// </summary>
    public class InvestorService : IInvestorService
    {
        private readonly IInvestorRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<InvestorService> _logger;

        /// <summary>
        /// Constructor with dependency injection of repository, AutoMapper, and logger.
        /// </summary>
        public InvestorService(IInvestorRepository repo, IMapper mapper, ILogger<InvestorService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvestorDto>> GetAllAsync()
        {
            try
            {
                var entities = await _repo.GetAllAsync();
                _logger.LogInformation("Retrieved {Count} investors", entities.Count());
                return _mapper.Map<IEnumerable<InvestorDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all investors");
                throw new ApplicationException("Unable to retrieve investors at this time.", ex);
            }
        }

        public async Task<InvestorDto?> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Investor with ID {InvestorId} not found", id);
                    return null;
                }

                _logger.LogInformation("Retrieved investor {InvestorId}", id);
                return _mapper.Map<InvestorDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving investor with ID {InvestorId}", id);
                throw new ApplicationException($"Unable to retrieve investor {id}.", ex);
            }
        }

        public async Task<InvestorDto> CreateAsync(InvestorCreateDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Attempted to create an investor with null DTO");
                throw new ArgumentNullException(nameof(dto));
            }

            try
            {
                var entity = _mapper.Map<Investor>(dto);
                await _repo.AddAsync(entity);
                await _repo.SaveChangesAsync();

                _logger.LogInformation("Created new investor {InvestorId} with name {Name}", entity.InvestorId, entity.FullName);
                return _mapper.Map<InvestorDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating investor {@InvestorDto}", dto);
                throw new ApplicationException("Unable to create investor.", ex);
            }
        }

        public async Task UpdateAsync(InvestorUpdateDto dto)
        {
            try
            {
                var entity = _mapper.Map<Investor>(dto);
                await _repo.UpdateAsync(entity);
                await _repo.SaveChangesAsync();

                _logger.LogInformation("Updated investor {InvestorId}", entity.InvestorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating investor {@InvestorUpdateDto}", dto);
                throw new ApplicationException($"Unable to update investor {dto.InvestorId}.", ex);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                await _repo.DeleteAsync(id);
                await _repo.SaveChangesAsync();

                _logger.LogInformation("Deleted investor {InvestorId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting investor {InvestorId}", id);
                throw new ApplicationException($"Unable to delete investor {id}.", ex);
            }
        }

    }
}
