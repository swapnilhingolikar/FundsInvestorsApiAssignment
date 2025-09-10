using Microsoft.EntityFrameworkCore;
using FundsInvestorsApi.Models;

namespace FundsInvestorsApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Represents tables in the database.
        /// </summary>
        
        public DbSet<Fund> Funds { get; set; } = null!;
        public DbSet<Investor> Investors { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;


        /// <summary>
        /// Configures entity relationships, keys, and property constraints.
        /// </summary>
        /// <param name="modelBuilder">ModelBuilder instance used to configure entities.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Primary Keys
            modelBuilder.Entity<Fund>().HasKey(f => f.FundId);
            modelBuilder.Entity<Investor>().HasKey(i => i.InvestorId);
            modelBuilder.Entity<Transaction>().HasKey(t => t.TransactionId);

            // Relationships
            modelBuilder.Entity<Investor>()
                .HasOne(i => i.Fund)                // Each Investor belongs to one Fund
                .WithMany(f => f.Investors)         // A Fund can have many Investors
                .HasForeignKey(i => i.FundId)       // Foreign key
                .OnDelete(DeleteBehavior.Cascade);  // Deleting a Fund deletes its Investors

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Investor)            // Each Transaction belongs to one Investor
                .WithMany(i => i.Transactions)      // An Investor can have many Transactions
                .HasForeignKey(t => t.InvestorId)   // Foreign key
                .OnDelete(DeleteBehavior.Cascade);  // Deleting an Investor deletes their Transactions

            // Property configurations
            modelBuilder.Entity<Fund>().Property(f => f.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Fund>().Property(f => f.Currency)
                .HasMaxLength(3) // ISO currency code (e.g., USD, INR, EUR)
                .IsRequired();

            modelBuilder.Entity<Investor>().Property(i => i.FullName)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<Investor>().Property(i => i.Email)
                .IsRequired();

            modelBuilder.Entity<Transaction>().Property(t => t.Amount)
                .HasColumnType("decimal(18,2)") // Precision for monetary values
                .IsRequired();
        }
    }
}
