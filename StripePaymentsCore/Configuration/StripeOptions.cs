namespace StripePaymentsCore.Configuration
{
    public class StripeOptions
    {
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
        public string WebHookSecretKey { get; set; }
    }
}