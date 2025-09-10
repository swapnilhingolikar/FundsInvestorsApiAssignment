using Microsoft.EntityFrameworkCore;
using FundsInvestorsApi.Models;

namespace FundsInvestorsApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Fund> Funds { get; set; } = null!;
        public DbSet<Investor> Investors { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Fund>().HasKey(f => f.FundId);
            modelBuilder.Entity<Investor>().HasKey(i => i.InvestorId);
            modelBuilder.Entity<Transaction>().HasKey(t => t.TransactionId);

            modelBuilder.Entity<Investor>()
                .HasOne(i => i.Fund)
                .WithMany(f => f.Investors)
                .HasForeignKey(i => i.FundId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Investor)
                .WithMany(i => i.Transactions)
                .HasForeignKey(t => t.InvestorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Fund>().Property(f => f.Name).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Fund>().Property(f => f.Currency).HasMaxLength(3).IsRequired();
            modelBuilder.Entity<Investor>().Property(i => i.FullName).HasMaxLength(150).IsRequired();
            modelBuilder.Entity<Investor>().Property(i => i.Email).IsRequired();
            modelBuilder.Entity<Transaction>().Property(t => t.Amount).HasColumnType("decimal(18,2)").IsRequired();
        }
    }
}
