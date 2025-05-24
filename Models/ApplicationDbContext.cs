using Microsoft.EntityFrameworkCore;

namespace InvoiceApp.Models
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.SellingPrice)
                .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.TotalSellingCost)
                .HasColumnType("decimal(18, 2)");

            // Configure relationships if necessary
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany() // No navigation property on Product
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoice>()
                .HasMany(i => i.OrderItems)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
