using ChineseFamilyRestaurant.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace ChineseFamilyRestaurant.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<ProductIngredient> ProductIngredients { get; set; }

        //ModelBuilder
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ProductIngredient entity
            modelBuilder.Entity<ProductIngredient>(entity =>
            {
                // Define composite primary key
                entity.HasKey(pi => new { pi.ProductId, pi.IngredientId });

                // Configure relationships
                entity.HasOne(pi => pi.Product)
                      .WithMany(p => p.ProductIngredients)
                      .HasForeignKey(pi => pi.ProductId); 

                entity.HasOne(pi => pi.Ingredient)
                      .WithMany(i => i.ProductIngredients)
                      .HasForeignKey(pi => pi.IngredientId);
            });

            //Seed Data 
            modelBuilder.Entity<Category>()
                .HasData(
                    new Category { CategoryId = 1, Name = "Appetizer"},
                    new Category { CategoryId = 2, Name = "Entree"},
                    new Category { CategoryId = 3, Name = "Side Dish" }, 
                    new Category { CategoryId = 4, Name = "Dessert"}, 
                    new Category { CategoryId = 5, Name = "Beverage"}
                );

            modelBuilder.Entity<Ingredient>().HasData(
                new Ingredient { IngredientId = 1, Name = "Beef", Description = "Fresh and tender beef" },
                new Ingredient { IngredientId = 2, Name = "Chicken", Description = "Organic chicken" },
                new Ingredient { IngredientId = 3, Name = "Fish", Description = "Wild-caught fish" },
                new Ingredient { IngredientId = 4, Name = "Tortilla", Description = "Soft flour tortilla" },
                new Ingredient { IngredientId = 5, Name = "Lettuce", Description = "Crisp and fresh lettuce" },
                new Ingredient { IngredientId = 6, Name = "Tomato", Description = "Ripe and juicy tomatoes" }
            );

            modelBuilder.Entity<Product>()
                .HasData(
                    new Product
                    {
                        ProductId = 1,
                        Name = "Beef Taco",
                        Description = "A Delicious Beef Taco",
                        Price = 2.50m,
                        Stock = 100,
                        CategoryId = 2
                    },
                    new Product
                    {
                        ProductId = 2,
                        Name = "Chicken Taco",
                        Description = "A Delicious Chicken Taco",
                        Price = 1.99m,
                        Stock = 150,
                        CategoryId = 2
                    },
                    new Product
                    {
                        ProductId = 3,
                        Name = "Fish Taco",
                        Description = "A Delicious Fish Taco",
                        Price = 3.99m,
                        Stock = 300,
                        CategoryId = 2
                    }
                );

            modelBuilder.Entity<ProductIngredient>()
                .HasData(
                    new ProductIngredient { ProductId = 1, IngredientId = 1},
                    new ProductIngredient { ProductId = 1, IngredientId = 4},
                    new ProductIngredient { ProductId = 1, IngredientId = 5},
                    new ProductIngredient { ProductId = 1, IngredientId = 6},
                    new ProductIngredient { ProductId = 2, IngredientId = 2},
                    new ProductIngredient { ProductId = 2, IngredientId = 4},
                    new ProductIngredient { ProductId = 2, IngredientId = 5},
                    new ProductIngredient { ProductId = 2, IngredientId = 6},
                    new ProductIngredient { ProductId = 3, IngredientId = 3},
                    new ProductIngredient { ProductId = 3, IngredientId = 4},
                    new ProductIngredient { ProductId = 3, IngredientId = 5},
                    new ProductIngredient { ProductId = 3, IngredientId = 6}
                );
        }
    }
}
