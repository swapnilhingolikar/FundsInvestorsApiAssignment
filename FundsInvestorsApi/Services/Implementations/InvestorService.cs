using AutoMapper;
using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Models;
using FundsInvestorsApi.Repositories;

namespace FundsInvestorsApi.Services
{
    public class InvestorService : IInvestorService
    {
        private readonly IInvestorRepository _repo;
        private readonly IMapper _mapper;

        public InvestorService(IInvestorRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InvestorDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<InvestorDto>>(entities);
        }

        public async Task<InvestorDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return _mapper.Map<InvestorDto?>(entity);
        }

        public async Task<InvestorDto> CreateAsync(InvestorCreateDto dto)
        {
            var entity = _mapper.Map<Investor>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return _mapper.Map<InvestorDto>(entity);
        }

        public async Task UpdateAsync(InvestorUpdateDto dto)
        {
            var entity = _mapper.Map<Investor>(dto);
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
