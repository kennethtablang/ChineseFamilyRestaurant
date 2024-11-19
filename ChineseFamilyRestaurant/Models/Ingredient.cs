namespace ChineseFamilyRestaurant.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<ProductIngredient> ProductIngredients { get; set; }
    }
}