using Microsoft.EntityFrameworkCore;
using ShoeStoreBackend.Data;
using ShoeStoreBackend.DTOs;
using ShoeStoreBackend.Models;

namespace ShoeStoreBackend.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CartItem>> GetCartItemsAsync(int userId)
        {
            return await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task AddToCartAsync(int userId, CartItemDto item)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == item.ProductId);

            if (cartItem != null)
            {
                cartItem.Quantity += item.Quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                _context.CartItems.Add(cartItem);
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(int userId, int productId, int quantity)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCartAsync(int userId, int productId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}