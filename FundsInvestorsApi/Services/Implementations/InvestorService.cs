using AutoMapper;
using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Models;
using FundsInvestorsApi.Repositories;

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

        /// <summary>
        /// Constructor with dependency injection of repository and AutoMapper.
        /// </summary>
        /// <param name="repo">Investor repository for data access.</param>
        /// <param name="mapper">AutoMapper instance for mapping between DTOs and entities.</param>
        public InvestorService(IInvestorRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all investors from the repository.
        /// </summary>
        /// <returns>List of investors as DTOs.</returns>
        public async Task<IEnumerable<InvestorDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync(); // Fetch all investors
            return _mapper.Map<IEnumerable<InvestorDto>>(entities); // Map to DTO list
        }

        /// <summary>
        /// Retrieves a single investor by ID.
        /// </summary>
        /// <param name="id">Unique identifier of the investor.</param>
        /// <returns>Investor details if found, otherwise null.</returns>
        public async Task<InvestorDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id); // Fetch by ID
            return _mapper.Map<InvestorDto?>(entity);  // Map to DTO
        }

        /// <summary>
        /// Creates a new investor and saves it in the repository.
        /// </summary>
        /// <param name="dto">Investor details for creation.</param>
        /// <returns>Created investor as DTO.</returns>
        public async Task<InvestorDto> CreateAsync(InvestorCreateDto dto)
        {
            var entity = _mapper.Map<Investor>(dto); // Map DTO → entity
            await _repo.AddAsync(entity);            // Add to repo
            await _repo.SaveChangesAsync();          // Commit changes
            return _mapper.Map<InvestorDto>(entity); // Map back → DTO
        }

        /// <summary>
        /// Updates existing investor details.
        /// </summary>
        /// <param name="dto">Updated investor details.</param>
        public async Task UpdateAsync(InvestorUpdateDto dto)
        {
            var entity = _mapper.Map<Investor>(dto); // Map DTO → entity
            await _repo.UpdateAsync(entity);         // Update in repo
            await _repo.SaveChangesAsync();          // Commit changes
        }

        /// <summary>
        /// Deletes an investor by ID.
        /// </summary>
        /// <param name="id">Investor ID.</param>
        public async Task DeleteAsync(Guid id)
        {
            await _repo.DeleteAsync(id);     // Remove from repo
            await _repo.SaveChangesAsync();  // Commit changes
        }
    }
}
