using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Responses;

public class PostPaymentResponse
{
    
    public Guid Id { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentStatus Status { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string CardNumberLastFour { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int ExpiryMonth { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int ExpiryYear { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Currency { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Amount { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ErrorMessage { get; set; }
}
