﻿using System.Text.Json.Serialization;

namespace E_Commerce.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public string ImageUrl { get; set; }
    }
}
