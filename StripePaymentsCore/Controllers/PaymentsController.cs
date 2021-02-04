using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using StripePaymentsCore.API;
using StripePaymentsCore.Configuration;

namespace StripePaymentsCore.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IStripeProvider _stripeProvider;
        private readonly StripeClient _client;

        public PaymentsController(IStripeProvider stripeProvider)
        {
            _stripeProvider = stripeProvider;
            _client = new StripeClient(_stripeProvider?.GetStripeOptions().SecretKey);

        }

        [HttpGet("public-keys")]
        public ActionResult<StripeOptions> GetPublicKeys()
        {
            return new StripeOptions
            {
                PublishableKey = _stripeProvider?.GetStripeOptions().PublishableKey
            };
        }

        [HttpPost("create-checkout-session")]
        public ActionResult CreateCheckoutSession()
        {
            try
            {
                // Get Product from Stripe e.g. "Funding Apple FCU Account"
                var product = _stripeProvider?.GetProduct().Result;

                // Get Price from Stripe e.g. "Funding Apple FCU Account Price"
                var price = _stripeProvider?.GetPrice().Result;

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                    {
                        "card",
                    },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {

                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = price ?.UnitAmount,
                                Currency = price ?.Currency,
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = product ?.Name,
                                    Images = product ?.Images
                                },
                            },
                            Quantity = 1, 
                        }, 
                    },
                    Mode = "payment",
                    SuccessUrl = "https://example.com/success",
                    CancelUrl = "https://example.com/cancel",
                };
                var sessionService = new SessionService(_client);
                var session = sessionService?.Create(options);
                Console.WriteLine($"Checkout Session with ID {session?.Id} Completed");
                return Json(new { id = session?.Id });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
 