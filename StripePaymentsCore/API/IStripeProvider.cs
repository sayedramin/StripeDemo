using System.Threading.Tasks;
using Stripe;
using StripePaymentsCore.Configuration;

namespace StripePaymentsCore.API
{
    public interface IStripeProvider
    {
        public Task<Product> GetProduct();
        public Task<Price> GetPrice();
        public StripeOptions GetStripeOptions();
    }
}