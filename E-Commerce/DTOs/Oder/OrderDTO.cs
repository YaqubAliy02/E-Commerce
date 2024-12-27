using E_Commerce.DTOs.Customer;
using E_Commerce.DTOs.OrderItem;

namespace E_Commerce.DTOs.Oder
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public CustomerDTO Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
    }
}
