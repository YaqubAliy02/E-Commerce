﻿using E_Commerce.DTOs.Category;

namespace E_Commerce.DTOs.Product
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public CategoryDTO Category { get; set; }
    }
}
