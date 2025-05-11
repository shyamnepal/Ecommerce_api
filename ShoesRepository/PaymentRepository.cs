using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ShoesShared.ModelDto;
using Stripe;

namespace ShoesRepository
{
    public class PaymentRepository: IPaymentRepository
    {
        private readonly IConfiguration _config;

        public PaymentRepository(IConfiguration config)
        {
            _config = config;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public async Task<PaymentIntentDto> CreateOrUpdatePaymentIntent(string basketId)
        {
            // Normally you'd calculate this based on basket contents
            var amount = 5000; // $50.00 in cents

            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
                Metadata = new Dictionary<string, string>
            {
                { "basketId", basketId }
            }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            // Return both the PaymentIntent ID and the ClientSecret
            
            return new PaymentIntentDto
            {
                Id = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret
               
            };

        }

       
    }
}
