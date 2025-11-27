using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.DAL
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Drink> Drinks { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship between MenuItem and Category
            modelBuilder.Entity<MenuItem>()
                .HasOne(m => m.Category) // A MenuItem has one Category
                .WithMany() // A Category can have many MenuItems
                .HasForeignKey(m => m.CategoryId) // Foreign key in MenuItem
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

            // Fix ApplicationUser and Drink relationships
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.FavoriteDrinks)
                .WithMany()
                .UsingEntity(j => j.ToTable("UserFavoriteDrinks"));

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.CreatedDrinks)
                .WithOne(d => d.CreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure decimal properties to avoid truncation
            modelBuilder.Entity<Drink>()
                .Property(d => d.BasePrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Drink>()
                .Property(d => d.SalePrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Ingredient>()
                .Property(i => i.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<MenuItem>()
                .Property(m => m.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}