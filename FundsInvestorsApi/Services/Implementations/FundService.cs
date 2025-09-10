using AutoMapper;
using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Models;
using FundsInvestorsApi.Repositories;

namespace FundsInvestorsApi.Services
{
    public class FundService : IFundService
    {
        private readonly IFundRepository _repo;
        private readonly IMapper _mapper;

        public FundService(IFundRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FundDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<FundDto>>(entities);
        }

        public async Task<FundDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return _mapper.Map<FundDto?>(entity);
        }

        public async Task<FundDto> CreateAsync(FundCreateDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }
            var entity = _mapper.Map<Fund>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return _mapper.Map<FundDto>(entity);
        }

        public async Task UpdateAsync(FundUpdateDto dto)
        {
            var entity = _mapper.Map<Fund>(dto);
            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
        }
    }
}
