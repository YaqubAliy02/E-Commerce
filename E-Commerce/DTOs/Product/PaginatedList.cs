﻿namespace E_Commerce.DTOs.Product
{
    public class PaginatedList<T>
    {
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}

