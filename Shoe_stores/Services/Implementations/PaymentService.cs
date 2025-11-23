using Microsoft.EntityFrameworkCore;
using ShoeStoreBackend.Data;
using Stripe;
using ShoeStoreBackend.Services.Interfaces; // Thêm dòng này
using System.Threading.Tasks;

namespace ShoeStoreBackend.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public PaymentService(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
            
        }

        public async Task<string?> CreatePaymentIntentAsync(PaymentRequest request)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = request.Amount,
                Currency = request.Currency,
                Description = request.Description,
                Metadata = new Dictionary<string, string> { { "OrderId", request.OrderId.ToString() } },
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);
            return paymentIntent.ClientSecret;
        }

        public async Task UpdateOrderPaymentStatusAsync(string paymentIntentId, string status)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntentId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
        }
    }
}