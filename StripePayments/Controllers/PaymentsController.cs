// This example sets up an endpoint using the ASP.NET MVC framework.
// Watch this video to get started: https://youtu.be/2-mMOB8MhmE.

using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Internal;
using System.Runtime.InteropServices.ComTypes;
using System.Web.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace server.Controllers
{
    public class PaymentsController : Controller
    {
        public PaymentsController()
        {
            // Set your secret key. Remember to switch to your live secret key in production.
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.ApiKey =
                "sk_test_51HL8TALEDdIklrMM8PLbLtDYBXFlVMkZnK6R7yRmpl4jjbJMLrC8cpF1PfkLkQrhTsz8RFzNA6bhwuAgNIed3BQX00AfFvhAWw";
        }

        //[System.Web.Mvc.HttpPost("create-checkout-session")]
        public ActionResult CreateCheckoutSession()
        {
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
                            UnitAmount = 2000,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "T-shirt",
                            },

                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = "https://example.com/success",
                CancelUrl = "https://example.com/cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Json(new { id = session.Id });
        }
    }
}