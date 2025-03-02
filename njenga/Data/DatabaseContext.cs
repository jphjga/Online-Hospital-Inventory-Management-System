using Microsoft.EntityFrameworkCore;
using Njenga.Models;

namespace Njenga.Data
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<Account> Accounts { get; set; } // Plural name here
        public DbSet<PurchaseRecord> PurchaseRecords { get; set; }
        public DbSet<StockTaking> StockTakings { get; set; }
        public DbSet<SalesRecord> SalesRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure correct table mapping
            modelBuilder.Entity<Product>().ToTable("products");
            modelBuilder.Entity<Account>().ToTable("account"); // Ensure matches DB
            modelBuilder.Entity<Institution>().ToTable("institutions"); // Added
            modelBuilder.Entity<StockTaking>().ToTable("stocktaking");
        }
    }
}
