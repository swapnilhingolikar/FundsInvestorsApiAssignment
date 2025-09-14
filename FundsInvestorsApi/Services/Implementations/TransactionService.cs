using AutoMapper;
using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Models;
using FundsInvestorsApi.Repositories;
using Microsoft.Extensions.Logging;

namespace FundsInvestorsApi.Services
{
    /// <summary>
    /// Service class responsible for handling transaction-related business logic.
    /// It acts as a bridge between the Controller and Repository layers.
    /// Uses AutoMapper for mapping between DTOs and entity models.
    /// </summary>
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<TransactionService> _logger;

        /// <summary>
        /// Constructor with dependency injection of repository, AutoMapper, and logger.
        /// </summary>
        public TransactionService(ITransactionRepository repo, IMapper mapper, ILogger<TransactionService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TransactionDto>> GetAllAsync()
        {
            try
            {
                var entities = await _repo.GetAllAsync();
                _logger.LogInformation("Retrieved {Count} transactions", entities.Count());
                return _mapper.Map<IEnumerable<TransactionDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all transactions");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TransactionDto?> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Transaction with ID {TransactionId} not found", id);
                    return null;
                }

                _logger.LogInformation("Retrieved transaction {TransactionId}", id);
                return _mapper.Map<TransactionDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction with ID {TransactionId}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TransactionDto> CreateAsync(TransactionCreateDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Attempted to create a transaction with null DTO");
                throw new ArgumentNullException(nameof(dto));
            }

            try
            {
                var entity = _mapper.Map<Transaction>(dto);
                await _repo.AddAsync(entity);
                await _repo.SaveChangesAsync();

                _logger.LogInformation("Created new transaction {TransactionId} for Investor {InvestorId} ",
                    entity.TransactionId, entity.InvestorId);

                return _mapper.Map<TransactionDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction {@TransactionDto}", dto);
                throw;
            }
        }
    }
}
