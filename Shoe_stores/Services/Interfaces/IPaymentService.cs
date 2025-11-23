using System.Threading.Tasks;

namespace ShoeStoreBackend.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<string?> CreatePaymentIntentAsync(PaymentRequest request);
        Task UpdateOrderPaymentStatusAsync(string paymentIntentId, string status);
    }

    public class PaymentRequest
    {
        public long OrderId { get; set; }
        public long Amount { get; set; }
        public string? Currency { get; set; }
        public string? Description { get; set; }
    }
}