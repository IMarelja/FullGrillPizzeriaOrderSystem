using Microsoft.EntityFrameworkCore;
using Models;

namespace GrillPizzeriaOrderMiddleware.DatabaseContexts
{
    public class GrillPizzaDatabaseContext : DbContext
    {
        public GrillPizzaDatabaseContext(DbContextOptions<GrillPizzaDatabaseContext> context) 
            : base(context) {}

        public DbSet<Log> Log { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Food> Food { get; set; }
        public DbSet<OrderFood> OrderFood { get; set; }
        public DbSet<Allergen> Allergen { get; set; }
        public DbSet<FoodCategory> FoodCategory { get; set; }
        public DbSet<FoodAllergen> FoodAllergen { get; set; }
        public DbSet<Role> Role { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Tables (M:N)

            modelBuilder.Entity<OrderFood>()
                .HasKey(of => new { of.OrderId, of.FoodId });

            modelBuilder.Entity<OrderFood>()
                .HasOne(of => of.Order)
                .WithMany(o => o.OrderFoods)
                .HasForeignKey(of => of.OrderId);

            modelBuilder.Entity<OrderFood>()
                .HasOne(of => of.Food)
                .WithMany(f => f.OrderFoods)
                .HasForeignKey(of => of.FoodId);

            modelBuilder.Entity<FoodAllergen>()
                .HasKey(fa => new { fa.FoodId, fa.AllergenId });

            modelBuilder.Entity<FoodAllergen>()
                .HasOne(fa => fa.Food)
                .WithMany(f => f.FoodAllergens)
                .HasForeignKey(fa => fa.FoodId);

            modelBuilder.Entity<FoodAllergen>()
                .HasOne(fa => fa.Allergen)
                .WithMany(a => a.FoodAllergens)
                .HasForeignKey(fa => fa.AllergenId);


            // Tables (1:N)

            modelBuilder.Entity<Food>()
                .HasOne(f => f.FoodCategory)
                .WithMany(fc => fc.Foods)
                .HasForeignKey(f => f.FoodCategoryId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);
        }
    }
}
