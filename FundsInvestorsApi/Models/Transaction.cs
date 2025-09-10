using System;

namespace FundsInvestorsApi.Models
{
    public enum TransactionType
    {
        Subscription,
        Redemption
    }

    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public Guid InvestorId { get; set; }
        public Investor? Investor { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}