using Microsoft.AspNetCore.Mvc;
using ShoeStoreBackend.Services.Interfaces;
using Stripe;
using System.IO;
using System.Threading.Tasks;

namespace ShoeStoreBackend.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly string _endpointSecret = ""; 

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _endpointSecret);

                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent != null)
                    {
                        await _paymentService.UpdateOrderPaymentStatusAsync(paymentIntent.Id, "Paid");
                    }
                }
                else if (stripeEvent.Type == "payment_intent.failed")
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent != null)
                    {
                        await _paymentService.UpdateOrderPaymentStatusAsync(paymentIntent.Id, "Failed");
                    }
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}