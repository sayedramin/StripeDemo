using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Stripe;
using StripePaymentsCore.Configuration;

namespace StripePaymentsCore.API
{
    public class StripeProvider : IStripeProvider
    {
        private readonly IOptions<StripeOptions> _options;
        private readonly StripeClient _client;

        public StripeProvider(IOptions<StripeOptions> options)
        {
            _options = options;
            _client = new StripeClient(options.Value.SecretKey);
        }
        public StripeOptions GetStripeOptions()
        {
            return new StripeOptions
            {
                SecretKey = _options.Value.SecretKey,
                PublishableKey = _options.Value.PublishableKey,
                WebHookSecretKey = _options.Value.WebHookSecretKey
            };
        }
        public async Task<Product> GetProduct()
        {
            var productService = new ProductService(_client);
            return await productService.GetAsync("prod_Ikga0LTBshqm23");
        }
        public async Task<Price> GetPrice()
        {
            var priceService = new PriceService(_client);
            var price = await priceService.GetAsync("price_1I9BDlLEKfRfXn5LnzigkJpT");
            return price;
        }
    }
}
