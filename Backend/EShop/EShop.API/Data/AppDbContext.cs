using EShop.Domain.Entities;
using EShop.Domain.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EShop.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Buyer> Buyers => Set<Buyer>();
    public DbSet<Seller> Sellers => Set<Seller>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // TPT inheritance
        builder.Entity<AuthorizedUser>().ToTable("AuthorizedUsers");
        builder.Entity<Buyer>().ToTable("Buyers");
        builder.Entity<Seller>().ToTable("Sellers");

        // OrderTotalPrice is computed in C# only — not stored in DB
        builder.Entity<Order>().Ignore(o => o.OrderTotalPrice);
    }
}
