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
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(_connString);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProductDetails>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()")
                    .ValueGeneratedOnAdd();
            });
        }
    }
}
