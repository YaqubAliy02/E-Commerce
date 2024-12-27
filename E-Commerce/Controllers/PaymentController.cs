using E_Commerce.Data;
using E_Commerce.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ECommerceDbContext _context;
        private readonly PaymentSettings _paymentSettings;

        public PaymentController(ECommerceDbContext context, IOptions<PaymentSettings> paymentSettings)
        {
            _context = context;
            _paymentSettings = paymentSettings.Value;
            StripeConfiguration.ApiKey = _paymentSettings.SecretKey;
        }

        // POST: api/Payments/charge
        [HttpPost("charge")]
        public async Task<IActionResult> ProcessPayment(int orderId)
        {
            var order = await _context.Orders.Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound("Order not found.");

            if (order.PaymentStatus == "Paid")
                return BadRequest("Order is already paid.");

            try
            {
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
                {
                    Amount = (long)(order.TotalAmount * 100), // Convert to cents
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" },
                });

                // Save the payment status as successful
                order.PaymentStatus = "Paid";
                _context.Entry(order).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Payment processed successfully.",
                    PaymentIntentId = paymentIntent.Id
                });
            }
            catch (StripeException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}