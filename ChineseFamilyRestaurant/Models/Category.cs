﻿namespace ChineseFamilyRestaurant.Models
{
    public class Category
    {
        public int CatregoryId { get; set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; } 
    }
}