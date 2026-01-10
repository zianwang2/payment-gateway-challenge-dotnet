using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Models.Requests;

public class PostPaymentRequest : IValidatableObject
{
    [Required]
    [RegularExpression(@"^\d{14,19}$", ErrorMessage = "Must be between 14 and 19 characters long and only contain numeric characters")]
    public string CardNumber { get; set; }
    
    [Required]
    [Range(1, 12, ErrorMessage = "Value must be between 1-12")]
    public int ExpiryMonth { get; set; }
    
    [Required]
    [Range(2000, 9999)]
    public int ExpiryYear { get; set; }
    
    [Required]
    [RegularExpression(@"^.{3}$", ErrorMessage = "Must be 3 characters")]
    [AllowedIsoCurrencyCode]
    public string Currency { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int Amount { get; set; }
    
    [Required]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Must be 3-4 characters long and only contain numeric characters")]
    public string Cvv { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        var now = DateTime.UtcNow;

        var currentMonth = new DateTime(now.Year, now.Month, 1);
        var expiryMonth = new DateTime(ExpiryYear, ExpiryMonth, 1);
        
        if (expiryMonth < currentMonth)
        {
            yield return new ValidationResult(
                "The combination of expiry month and year must be in the future",
                new[] { nameof(ExpiryMonth), nameof(ExpiryYear) }
            );
        }
    }
}