using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TequilasRestaurant.Models
{
    public class Product
    {
        static readonly string profile_pic = "https://www.greendrop.com/assets/images/beautiful-tree-with-large-green-top.jpg";
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId {  get; set; }

        //This will not map the file to the database with Entity Relationship Document (ERD) migration
        //Instead, we'll bring over the ImageUrl
        [NotMapped] 
        public IFormFile? ImageFile { get; set; }
        public string ImageUrl { get; set; } = profile_pic;

        public Category? Category { get; set; } // A product belongs to a category
        public ICollection<OrderItem>? OrderItems { get; set; } // A product can be in many order items
        public ICollection<ProductIngredient>? ProductIngredients { get; set; } // A product can have many ingredients
    }
}
