using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TequilasRestaurant.Models;

namespace TequilasRestaurant.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        //? What do the lines below do?
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> Items { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<ProductIngredient> ProductIngredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //The code below has a key that is made up of 2 foreign keys { ProductId, IngredientId } aka composite key
            //Its goal is to create the some relationships between tables shown on the Entity Relationship Document (EDR)
            modelBuilder.Entity<ProductIngredient>().HasKey(pi => new { pi.ProductId, pi.IngredientId });

            modelBuilder.Entity<ProductIngredient>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductIngredients)
                .HasForeignKey(pi => pi.ProductId);

            modelBuilder.Entity<ProductIngredient>()
                .HasOne(pi => pi.Ingredient)
                .WithMany(i => i.ProductIngredients)
                .HasForeignKey(pi => pi.IngredientId);

            //? What is a seed data?
            // Adding some seed data - categories of food
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Appetizer" },
                new Category { CategoryId = 2, Name = "Entree" },
                new Category { CategoryId = 3, Name = "Side Dish" },
                new Category { CategoryId = 4, Name = "Dessert" },
                new Category { CategoryId = 5, Name = "Beverage" }
                );

            // Adding some seed data - ingredient table
            modelBuilder.Entity<Ingredient>().HasData(
                new Ingredient { IngredientId = 1, Name = "Beef"},
                new Ingredient { IngredientId = 2, Name = "Chicken"},
                new Ingredient { IngredientId = 3, Name = "Fish"},
                new Ingredient { IngredientId = 4, Name = "Tortilla"},
                new Ingredient { IngredientId = 5, Name = "Lettuce"},
                new Ingredient { IngredientId = 6, Name = "Tomato"}
                );

            // Adding some seed data - product (actual food) table
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, Name = "Taco Beef", Description = "A delicious beef taco", Price = 2.50m, Stock = 100, CategoryId = 2 },
                new Product { ProductId = 2, Name = "Taco Chicken", Description = "A delicious chicken taco", Price = 2.50m, Stock = 101, CategoryId = 2 },
                new Product { ProductId = 3, Name = "Taco Fish", Description = "A delicious fish taco", Price = 2.50m, Stock = 90, CategoryId = 2 }
                );

            // wiring up - relating the ingredients to the food product #1
            modelBuilder.Entity<ProductIngredient>().HasData(
                new ProductIngredient { ProductId = 1, IngredientId = 1 },
                new ProductIngredient { ProductId = 1, IngredientId = 4 },
                new ProductIngredient { ProductId = 1, IngredientId = 5 },
                new ProductIngredient { ProductId = 1, IngredientId = 6 }
                );

            // wiring up - relating the ingredients to the food product #2
            modelBuilder.Entity<ProductIngredient>().HasData(
                new ProductIngredient { ProductId = 2, IngredientId = 2 },
                new ProductIngredient { ProductId = 2, IngredientId = 4 },
                new ProductIngredient { ProductId = 2, IngredientId = 5 },
                new ProductIngredient { ProductId = 2, IngredientId = 6 }
                );

            // wiring up - relating the ingredients to the food product #3
            modelBuilder.Entity<ProductIngredient>().HasData(
                new ProductIngredient { ProductId = 3, IngredientId = 3 },
                new ProductIngredient { ProductId = 3, IngredientId = 4 },
                new ProductIngredient { ProductId = 3, IngredientId = 5 },
                new ProductIngredient { ProductId = 3, IngredientId = 6 }
                );



        }
    }
}
