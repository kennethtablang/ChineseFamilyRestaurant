using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ChineseFamilyRestaurant.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }

        [Required(ErrorMessage = "Name is Required.")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string? Description { get; set; }

        [ValidateNever]
        public ICollection<ProductIngredient>? ProductIngredients { get; set; }
    }
}