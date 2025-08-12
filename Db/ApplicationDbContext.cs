using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StockNotificationWarning.Config;
using StockNotificationWarning.Entities;

namespace StockNotificationWarning.Db
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        IOptionsMonitor<DbConfig> dbMonitor) : DbContext(options)
    {
        public DbSet<ProductDetails> ProductDetails { get; set; }
        readonly string _connString = dbMonitor.CurrentValue.DefaultConnection;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(_connString);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProductDetails>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
            });

            builder.Entity<ProductDetails>().HasData(
            new ProductDetails
            {
                Id = 1,
                ShopifyProductGid = "gid://shopify/Product/8858179666170",
                CustomDisplayName = "My Newest Product",
                CustomDescription = "This is a detailed description for the newest product.",
            },
                new ProductDetails
                {
                    Id = 2,
                    ShopifyProductGid = "gid://shopify/Product/8858181271802",
                    CustomDisplayName = "Patike",
                    CustomDescription = "Comfortable sneakers for everyday use.",
                },
                new ProductDetails
                {
                    Id = 3,
                    ShopifyProductGid = "gid://shopify/Product/8861054828794",
                    CustomDisplayName = "Cizme Model X",
                    CustomDescription = "Premium boots for winter season.",
                },
                new ProductDetails
                {
                    Id = 4,
                    ShopifyProductGid = "gid://shopify/Product/8861090283770",
                    CustomDisplayName = "My New Low Stock Product",
                    CustomDescription = "Keep an eye on inventory for this one!",
                },
                new ProductDetails
                {
                    Id = 5,
                    ShopifyProductGid = "gid://shopify/Product/8861082714362",
                    CustomDisplayName = "Soko",
                    CustomDescription = "A classic design with modern comfort.",
                }
            );
        }
    }
}
