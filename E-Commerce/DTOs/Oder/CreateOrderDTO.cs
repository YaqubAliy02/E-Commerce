namespace E_Commerce.DTOs.Oder
{
    public class CreateOrderDTO
    {
        public int CustomerId { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public ICollection<CreateOrderDTO> OrderItems { get; set; }
    }
}