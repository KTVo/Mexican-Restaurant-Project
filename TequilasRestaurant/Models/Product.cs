using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TequilasRestaurant.Models
{
    public class Product
    {
        public Product()
        {
            //Declaring an empty list for storing ingredients of each product
            ProductIngredients = new List<ProductIngredient>();
        }

        static readonly string profile_pic = "https://www.greendrop.com/assets/images/beautiful-tree-with-large-green-top.jpg";
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId {  get; set; }

        // This will not map the file to the database with Entity Relationship Document (ERD) migration
        // Instead, we'll bring over the ImageUrl
        [NotMapped] 
        public IFormFile? ImageFile { get; set; }
        //To avoid unintend validation errors use [ValidateNever]
        //?When to use this?
        public string ImageUrl { get; set; } = profile_pic;
        [ValidateNever]
        public Category? Category { get; set; } // A product belongs to a category
        [ValidateNever]
        public ICollection<OrderItem>? OrderItems { get; set; } // A product can be in many order items
        [ValidateNever]
        public ICollection<ProductIngredient>? ProductIngredients { get; set; } // A product can have many ingredients
    }
}