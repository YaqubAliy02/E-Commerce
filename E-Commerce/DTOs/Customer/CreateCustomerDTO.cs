using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTOs.Customer
{
    public class CreateCustomerDTO
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
