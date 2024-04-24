using CozyToGo.Models;
using Microsoft.EntityFrameworkCore;

namespace CozyToGo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<DishIngredient> DishIngredients { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Owner> Owners { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Owner>().HasIndex(o => o.Email).IsUnique();
            modelBuilder.Entity<DishIngredient>()
            .HasOne(i => i.Dish)
            .WithMany(d => d.DishIngredients)
            .HasForeignKey(i => i.IdDish)
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DishIngredient>()
            .HasOne(i => i.Ingredient)
            .WithMany(d => d.DishIngredients)
            .HasForeignKey(i => i.IdIngredient)
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Restaurant>()
                .HasOne(r => r.Owner)
                .WithOne(o => o.Restaurant)
                .HasForeignKey<Restaurant>(r => r.IdOwner)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
