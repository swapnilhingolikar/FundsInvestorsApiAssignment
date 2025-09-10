using System;
using System.ComponentModel.DataAnnotations;

namespace FundsInvestorsApi.DTOs
{
    public enum TransactionType
    {
        Subscription,
        Redemption
    }

    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid InvestorId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class TransactionCreateDto
    {
        [Required]
        public Guid InvestorId { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Required, Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }

    public class TransactionUpdateDto
    {
        [Required]
        public Guid TransactionId { get; set; }

        [Required]
        public Guid InvestorId { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Required, Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}
