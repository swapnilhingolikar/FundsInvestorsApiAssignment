using System;
using System.ComponentModel.DataAnnotations;

namespace FundsInvestorsApi.DTOs
{
    public class FundDto
    {
        public Guid FundId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public DateTime LaunchDate { get; set; }
    }

    public class FundCreateDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(3)]
        public string Currency { get; set; } = string.Empty;

        public DateTime LaunchDate { get; set; } = DateTime.UtcNow;
    }

    public class FundUpdateDto
    {
        [Required]
        public Guid FundId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(3)]
        public string Currency { get; set; } = string.Empty;

        public DateTime LaunchDate { get; set; }
    }
}
