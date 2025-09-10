using AutoMapper;
using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Models;
using FundsInvestorsApi.Repositories;

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

        /// <summary>
        /// Constructor with dependency injection for repository and AutoMapper.
        /// </summary>
        /// <param name="repo">Fund repository for data access.</param>
        /// <param name="mapper">AutoMapper instance for object mapping.</param>
        public FundService(IFundRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all funds.
        /// </summary>
        /// <returns>List of funds as DTOs.</returns>
        public async Task<IEnumerable<FundDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();        // Fetch all funds
            return _mapper.Map<IEnumerable<FundDto>>(entities); // Map to DTOs
        }

        /// <summary>
        /// Retrieves a fund by its ID.
        /// </summary>
        /// <param name="id">Unique identifier of the fund.</param>
        /// <returns>Fund details if found, otherwise null.</returns>
        public async Task<FundDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);   // Fetch by ID
            return _mapper.Map<FundDto?>(entity);        // Map to DTO
        }

        /// <summary>
        /// Creates a new fund.
        /// </summary>
        /// <param name="dto">Fund details for creation.</param>
        /// <returns>Created fund as DTO.</returns>
        /// <exception cref="ArgumentNullException">Thrown when dto is null.</exception>
        public async Task<FundDto> CreateAsync(FundCreateDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto)); // Validate input
            }

            var entity = _mapper.Map<Fund>(dto);   // Map DTO → entity
            await _repo.AddAsync(entity);          // Add to repository
            await _repo.SaveChangesAsync();        // Commit changes
            return _mapper.Map<FundDto>(entity);   // Map back → DTO
        }

        /// <summary>
        /// Updates an existing fund.
        /// </summary>
        /// <param name="dto">Updated fund details.</param>
        public async Task UpdateAsync(FundUpdateDto dto)
        {
            var entity = _mapper.Map<Fund>(dto);   // Map DTO → entity
            await _repo.UpdateAsync(entity);       // Update repository
            await _repo.SaveChangesAsync();        // Commit changes
        }

        /// <summary>
        /// Deletes a fund by its ID.
        /// </summary>
        /// <param name="id">Fund ID.</param>
        public async Task DeleteAsync(Guid id)
        {
            await _repo.DeleteAsync(id);     // Remove from repository
            await _repo.SaveChangesAsync();  // Commit changes
        }
    }
}
