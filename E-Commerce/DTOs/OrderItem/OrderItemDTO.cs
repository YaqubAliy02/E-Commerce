using E_Commerce.DTOs.Product;

namespace E_Commerce.DTOs.OrderItem
{
    public class OrderItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ProductDTO Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
