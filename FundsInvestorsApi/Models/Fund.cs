namespace FundsInvestorsApi.Models
{
    public class Fund
    {
        public Guid FundId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public DateTime LaunchDate { get; set; }
        public ICollection<Investor>? Investors { get; set; }
    }
}