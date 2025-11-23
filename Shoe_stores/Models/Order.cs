using ShoeStoreBackend.Models;

namespace ShoeStoreBackend.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; } = "Processing";
        public string? PaymentIntentId { get; set; }

        public User? User { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}