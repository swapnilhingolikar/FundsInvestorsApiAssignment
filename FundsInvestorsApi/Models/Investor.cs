namespace FundsInvestorsApi.Models
{
    public class Investor
    {
        public Guid InvestorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid FundId { get; set; }
        public Fund? Fund { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
    }
}