using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Stripe;
using Stripe.Checkout;
using StripePaymentsCore.API;

namespace StripePaymentsCore.Controllers
{
    public class FuturePaymentsController : Controller
    {
        private readonly IStripeProvider _stripeProvider;
        private readonly StripeClient _client;

        public FuturePaymentsController(IStripeProvider stripeProvider)
        {
            _stripeProvider = stripeProvider;
            _client = new StripeClient(_stripeProvider.GetStripeOptions().SecretKey);
        }

        //[HttpPost("create-customer")]
        private ActionResult<Customer> CreateNewCustomer()
        {
            var options = new CustomerCreateOptions
            {
                Name = "Ramin Sadat",
                Email = "rsadat@applefcu.org",
                Phone = "+15717996452",
                Address = new AddressOptions
                {
                    Line1 = "8617 Wales Ct",
                    City = "Gainesville",
                    State = "VA",
                    PostalCode = "20155",
                    Country = "USA",
                },
                Description = "Future Payments",
            };
            var service = new CustomerService(_client);
            try
            {
                var customer = service.Create(options);
                //return Json(customer); Only if returning data to to

                return customer;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet("create-payment-intent")]
        public ActionResult CreateCheckoutSession()
        {

            var customer = CreateNewCustomer();

            try

            {
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                    {
                        "card",
                    }, 
                    //Mode = "payment",
                    Mode = "setup",
                    Customer = customer.Value.Id, //"cus_IjwJcLvcrS45TD",
                    SuccessUrl = "https://example.com/success",
                    CancelUrl = "https://example.com/cancel",
                };
                // post checkout session to stripe 
                var sessionService = new SessionService(_client);
                var checkOutSession = sessionService.Create(options);

                return Json(checkOutSession.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost("charge-customer")]
        public ActionResult<Charge> ChargeCustomer(string sessionId)
        {
            try
            {
                // Retrieve Session 

                var sessionService = new SessionService(_client);
                var session = sessionService.Get("cs_test_c1FLnMmlumB6lwsmH0ZjgwA3F0VWfMy9idVRTysO5adsZMbCKxSti3wUx3");

                // Retrieve Customer 

                var customerService = new CustomerService(_client);
                var customer = customerService.Get(session.CustomerId);

                // Retrieve SetupIntent

                var setupIntentService = new SetupIntentService(_client);
                var setupIntent = setupIntentService.Get(session.SetupIntentId);

                // Retrieve PaymentMethod

                var paymentMethodService = new PaymentMethodService(_client);
                var paymentMethod = paymentMethodService.Get(setupIntent.PaymentMethodId);


                var paymentIntentOptions = new PaymentIntentCreateOptions
                {
                    Amount = 2300,
                    Currency = "usd",
                    Confirm = true,
                    Customer = customer.Id,
                    PaymentMethod = paymentMethod.Id
                };
                var paymentIntentService = new PaymentIntentService(_client);
                var intent = paymentIntentService.Create(paymentIntentOptions);

                

                

                return Json(intent.ClientSecret);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
