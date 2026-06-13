using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<ProductOrder> ProductOrders { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>()
                .HasMany(u => u.AssignedOrders)
                .WithOne(po => po.AssignedToUser)
                .HasForeignKey(po => po.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure ProductOrder entity
            modelBuilder.Entity<ProductOrder>()
                .HasKey(po => po.Id);

            modelBuilder.Entity<ProductOrder>()
                .Property(po => po.OrderQuantityMeters)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProductOrder>()
                .Property(po => po.CostPerMeterPO)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProductOrder>()
                .Property(po => po.CostPerMeterProduction)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProductOrder>()
                .Property(po => po.TotalPOValue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProductOrder>()
                .Property(po => po.TotalProductionValue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProductOrder>()
                .Property(po => po.MetersDelivered)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProductOrder>()
                .Property(po => po.MetersToBeDelivered)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProductOrder>()
                .Property(po => po.AmountReceived)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProductOrder>()
                .Property(po => po.AmountToBeReceived)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProductOrder>()
                .Property(po => po.PendingPayments)
                .HasPrecision(18, 2);
        }
    }
}
