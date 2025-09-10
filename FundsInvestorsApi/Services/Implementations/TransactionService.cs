using AutoMapper;
using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Models;
using FundsInvestorsApi.Repositories;

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

        /// <summary>
        /// Constructor with dependency injection of repository and AutoMapper.
        /// </summary>
        /// <param name="repo">Transaction repository for data access.</param>
        /// <param name="mapper">AutoMapper instance for mapping between DTOs and entities.</param>
        public TransactionService(ITransactionRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all transactions.
        /// </summary>
        /// <returns>List of transactions as DTOs.</returns>
        public async Task<IEnumerable<TransactionDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync(); // Fetch all transactions from repository
            return _mapper.Map<IEnumerable<TransactionDto>>(entities); // Map entities to DTOs
        }

        /// <summary>
        /// Retrieves a single transaction by its unique identifier.
        /// </summary>
        /// <param name="id">Transaction ID.</param>
        /// <returns>Transaction details as DTO if found, otherwise null.</returns>
        public async Task<TransactionDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id); // Fetch transaction by ID
            return _mapper.Map<TransactionDto?>(entity); // Map to DTO (nullable if not found)
        }

        /// <summary>
        /// Creates a new transaction and saves it to the repository.
        /// </summary>
        /// <param name="dto">Transaction details for creation.</param>
        /// <returns>Created transaction as DTO.</returns>
        public async Task<TransactionDto> CreateAsync(TransactionCreateDto dto)
        {
            // Map DTO to entity model
            var entity = _mapper.Map<Transaction>(dto);

            // Add new transaction to repository
            await _repo.AddAsync(entity);

            // Persist changes to database
            await _repo.SaveChangesAsync();

            // Map back to DTO for response
            return _mapper.Map<TransactionDto>(entity);
        }
    }
}
