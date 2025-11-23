namespace ShoeStoreBackend.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public class OrderRequestDto
    {
        [Required]
        public string? PaymentMethod { get; set; }
    }

    public class OrderResponseDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public List<OrderItemDto>? Items { get; set; }
        public string? ClientSecret { get; set; }
        public string? Status { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; } // Sử dụng nullable để tránh CS8618
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}