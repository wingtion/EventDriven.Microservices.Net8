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

        // ---> BU KISMI EKLİYORUZ <---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Order tablosundaki TotalPrice alanı için: Toplam 18 basamak, virgülden sonra 2 basamak
            modelBuilder.Entity<Order>()
                .Property(x => x.TotalPrice)
                .HasColumnType("decimal(18,2)");

            // OrderItem tablosundaki Price alanı için
            modelBuilder.Entity<OrderItem>()
                .Property(x => x.Price)
                .HasColumnType("decimal(18,2)");

            base.OnModelCreating(modelBuilder);
        }
        // ---> BİTİŞ <---
    }
}