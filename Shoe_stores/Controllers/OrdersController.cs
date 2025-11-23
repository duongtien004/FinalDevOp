using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeStoreBackend.DTOs;
using ShoeStoreBackend.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoeStoreBackend.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.PaymentMethod))
            {
                return BadRequest(new { message = "Payment method is required." });
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User ID is null"));
            try
            {
                var result = await _orderService.PlaceOrderAsync(userId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{orderId}/confirm")]
        [Authorize]
        public async Task<IActionResult> ConfirmPayment(int orderId, [FromBody] PaymentConfirmationDto paymentDto)
        {
            if (paymentDto == null || string.IsNullOrEmpty(paymentDto.PaymentIntentId))
            {
                return BadRequest(new { message = "Payment intent ID is required." });
            }

            try
            {
                await _orderService.ConfirmPaymentAsync(orderId, paymentDto.PaymentIntentId);
                return Ok(new { message = "Thanh toán xác nhận thành công", status = "Paid" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User ID is null"));
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }
    }

    public class PaymentConfirmationDto
    {
        public string PaymentIntentId { get; set; }
    }
}