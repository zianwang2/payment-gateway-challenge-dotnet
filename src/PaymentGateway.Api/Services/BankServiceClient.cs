using PaymentGateway.Api.Models.Exceptions;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

/// <summary>
/// HTTP client for the external bank/payment provider.
/// </summary>
public class BankServiceClient : IBankServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BankServiceClient> _logger;
    private readonly string processPaymentPath = "/payments";

    public BankServiceClient(HttpClient httpClient, ILogger<BankServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ProcessPaymentResponse> ProcessPaymentAsync(ProcessPaymentRequest request)
    {
        _logger.LogInformation("Sending payment request to provider. Path={Path} CardLast4={CardLast4}", processPaymentPath, request.CardNumber[^4..]);

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsJsonAsync(processPaymentPath, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HTTP request to provider failed. Path={Path}", processPaymentPath);
            throw new ProviderException("Failed to connect to provider");
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Provider returned non-success status. StatusCode={StatusCode}", response.StatusCode);
            throw new ProviderException($"Provider returned non-success status. Status code: {response.StatusCode}");
        }

        ProcessPaymentResponse result;
        try
        {
            result = await response.Content.ReadFromJsonAsync<ProcessPaymentResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize provider response");
            throw new ProviderException("Invalid response from provider");
        }

        if (result == null)
        {
            _logger.LogError("Provider response deserialized to null");
            throw new ProviderException("Invalid response from provider");
        }

        _logger.LogInformation("Provider processed payment. Authorized={Authorized} HasAuthCode={HasAuthCode}", result.Authorized, !string.IsNullOrEmpty(result.AuthorizationCode));

        return result;
    }
}