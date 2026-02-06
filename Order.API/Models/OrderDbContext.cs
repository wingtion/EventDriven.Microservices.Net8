using Microsoft.EntityFrameworkCore;

namespace Order.API.Models
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Order>()
                .Property(x => x.TotalPrice)
                .HasColumnType("decimal(18,2)");

            
            modelBuilder.Entity<OrderItem>()
                .Property(x => x.Price)
                .HasColumnType("decimal(18,2)");

            base.OnModelCreating(modelBuilder);
        }
        
    }
}