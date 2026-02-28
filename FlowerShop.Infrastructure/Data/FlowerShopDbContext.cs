using FlowerShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Data
{
    public class FlowerShopDbContext : DbContext
    {
        public FlowerShopDbContext(DbContextOptions<FlowerShopDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Flower> Flowers { get; set; }
        public DbSet<FlowerImage> FlowerImages { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. User - Cart (1-1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();
            // 2. User - Address (1-n)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Addresses)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // 3. Category - Flower (1-n)
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Flowers)
                .WithOne(f => f.Category)
                .HasForeignKey(f => f.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Flower - FlowerImage (1-n)
            modelBuilder.Entity<Flower>()
                .HasMany(f => f.FlowerImages)
                .WithOne(i => i.Flower)
                .HasForeignKey(i => i.FlowerID)
                .OnDelete(DeleteBehavior.Cascade);

            // 5. Cart - CartItem (1-n)
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartID)
                .OnDelete(DeleteBehavior.Cascade);

            // 6. Order - OrderItem (1-n)
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            // 7. Order - Payment (1-1)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderID)
                .OnDelete(DeleteBehavior.Cascade);


            // Flower - OrderItem (1-n)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Flower)
                .WithMany(f => f.OrderItems)
                .HasForeignKey(oi => oi.FlowerID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
