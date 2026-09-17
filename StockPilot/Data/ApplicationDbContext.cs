using Microsoft.EntityFrameworkCore;
using StockPilot.Models;

namespace StockPilot.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Sale> Sales { get; set; }

        public DbSet<SaleItem> SaleItems { get; set; }
    }
}