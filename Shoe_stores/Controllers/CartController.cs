using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeStoreBackend.DTOs;
using ShoeStoreBackend.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoeStoreBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
            {
                throw new UnauthorizedAccessException("Không tìm thấy UserId trong token.");
            }
            return int.Parse(userIdStr);
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var items = await _cartService.GetCartItemsAsync(userId);
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartItemDto item)
        {
            var userId = GetUserId();
            await _cartService.AddToCartAsync(userId, item);
            return Ok(new { message = "Đã thêm vào giỏ hàng." });
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateCart(int productId, [FromBody] int quantity)
        {
            var userId = GetUserId();
            await _cartService.UpdateCartItemAsync(userId, productId, quantity);
            return Ok(new { message = "Cập nhật số lượng thành công." });
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = GetUserId();
            await _cartService.RemoveFromCartAsync(userId, productId);
            return Ok(new { message = "Đã xoá sản phẩm khỏi giỏ hàng." });
        }
    }
}