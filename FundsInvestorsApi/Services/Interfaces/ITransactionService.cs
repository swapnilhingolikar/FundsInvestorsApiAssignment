using FundsInvestorsApi.DTOs;

namespace FundsInvestorsApi.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDto>> GetAllAsync();
        Task<TransactionDto?> GetByIdAsync(Guid id);
        Task<TransactionDto> CreateAsync(TransactionCreateDto dto);
    }
}
