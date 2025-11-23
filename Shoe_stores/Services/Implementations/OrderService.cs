using Microsoft.EntityFrameworkCore;
using ShoeStoreBackend.Data;
using ShoeStoreBackend.DTOs;
using ShoeStoreBackend.Models;
using ShoeStoreBackend.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoeStoreBackend.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IPaymentService _paymentService;

        public OrderService(AppDbContext context, IPaymentService paymentService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }

        public async Task<OrderResponseDto> PlaceOrderAsync(int userId, OrderRequestDto request)
        {
            var cartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                throw new Exception("Giỏ hàng của bạn đang trống.");

            decimal totalAmount = cartItems.Sum(ci => ci.Product.Price * ci.Quantity);

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                PaymentMethod = request.PaymentMethod,
                TotalAmount = totalAmount,
                OrderItems = cartItems.Select(ci => new ShoeStoreBackend.Models.OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var paymentRequest = new PaymentRequest
            {
                OrderId = order.Id,
                Amount = (long)(totalAmount * 100), // Chuyển sang cent
                Currency = "vnd",
                Description = $"Order #{order.Id} for User {userId}"
            };

            string? clientSecret = await _paymentService.CreatePaymentIntentAsync(paymentRequest);
            if (clientSecret != null)
            {
                order.PaymentIntentId = clientSecret.Split('_')[0];
                await _context.SaveChangesAsync();
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return new OrderResponseDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod,
                Items = order.OrderItems?.Select(oi => new ShoeStoreBackend.DTOs.OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList(),
                ClientSecret = clientSecret,
                Status = order.Status
            };
        }

        public async Task<List<OrderResponseDto>> GetUserOrdersAsync(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            return orders.Select(order => new OrderResponseDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod,
                Items = order.OrderItems?.Select(oi => new ShoeStoreBackend.DTOs.OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList(),
                Status = order.Status
            }).ToList();
        }

        public Task ConfirmPaymentAsync(int orderId, string paymentIntentId)
        {
            throw new NotImplementedException();
        }
    }
}