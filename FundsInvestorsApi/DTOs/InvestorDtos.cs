using System;
using System.ComponentModel.DataAnnotations;

namespace FundsInvestorsApi.DTOs
{
    public class InvestorDto
    {
        public Guid InvestorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid FundId { get; set; }
    }

    public class InvestorCreateDto
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public Guid FundId { get; set; }
    }

    public class InvestorUpdateDto
    {
        [Required]
        public Guid InvestorId { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public Guid FundId { get; set; }
    }
}
