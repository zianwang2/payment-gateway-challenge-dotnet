namespace PaymentGateway.Api.Models.Repository;

public class PaymentData
{
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public string CardNumber { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
    public string AuthorizationCode { get; set; }
}