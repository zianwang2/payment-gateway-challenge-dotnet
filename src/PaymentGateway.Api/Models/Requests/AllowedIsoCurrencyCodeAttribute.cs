using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Models.Requests
{
    public class AllowedIsoCurrencyCodeAttribute : ValidationAttribute
    {

        private static readonly HashSet<string> AllowedIsoCurrencyCodes = new(StringComparer.OrdinalIgnoreCase)
        {
            "GBP",
            "USD",
            "EUR",
        };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currency = value as string;

            if (string.IsNullOrEmpty(currency) || !AllowedIsoCurrencyCodes.Contains(currency))
            {
                return new ValidationResult("Must be allowed ISO currency code");
            }
            return ValidationResult.Success;
        }
    }
}
