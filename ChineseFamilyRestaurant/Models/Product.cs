using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChineseFamilyRestaurant.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public string ImageUrl { get; set; } = "https://via.placeholder.com/150";

        public int CategoryId { get; set; }
        
        [ValidateNever]
        public Category? Category { get; set; } // a product belongs to another category
        
        [ValidateNever]
        public ICollection<OrderItem>? OrderItems { get; set; } //  a product can be in a many order items

        [ValidateNever]
        public ICollection<ProductIngredient>? ProductIngredients { get; set; } // a product can have many ingredients
    }
}