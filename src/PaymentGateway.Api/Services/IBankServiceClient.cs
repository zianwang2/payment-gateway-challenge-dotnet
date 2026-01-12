using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

/// <summary>
/// HTTP client for the external bank/payment provider.
/// </summary>
public interface IBankServiceClient
{
    Task<ProcessPaymentResponse> ProcessPaymentAsync(ProcessPaymentRequest request);
}

