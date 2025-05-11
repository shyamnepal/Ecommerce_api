using DataAcess.Entity.OrderAggregate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoesRepository;
using Stripe;


namespace Ecommerce_shoes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IOrderRepository _orderRepo;

        public PaymentsController(IConfiguration config, IOrderRepository orderRepo)
        {
            _config = config;
            _orderRepo = orderRepo;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeSignature = Request.Headers["Stripe-Signature"];
            var webhookSecret = _config["Stripe:WebhookSecret"];

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    webhookSecret
                );
            }
            catch (Exception ex)
            {
                return BadRequest($"Webhook error: {ex.Message}");
            }

            if (stripeEvent.Type == "payment_intent.succeeded") // Fix: Ensure Events is resolved from Stripe.Checkout
            {
                var intent = stripeEvent.Data.Object as PaymentIntent;
                var paymentIntentId = intent.Id;

                var order = await _orderRepo.GetOrderByPaymentIntentId(paymentIntentId);
                if (order != null)
                {
                    order.Status = OrderStatus.PaymentReceived;
                    await _orderRepo.UpdateOrder(order);
                }
            }

            return Ok();
        }
    }
}
