using ShoeStoreBackend.DTOs;
using ShoeStoreBackend.Models;

namespace ShoeStoreBackend.Services
{
    public interface ICartService
    {
        Task<List<CartItem>> GetCartItemsAsync(int userId);
        Task AddToCartAsync(int userId, CartItemDto item);
        Task UpdateCartItemAsync(int userId, int productId, int quantity);
        Task RemoveFromCartAsync(int userId, int productId);
    }
}