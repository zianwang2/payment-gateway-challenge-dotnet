using PaymentGateway.Api.Models.Exceptions;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class BankServiceClient : IBankServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly string processPaymentPath = "/payments";

    public BankServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ProcessPaymentResponse> ProcessPaymentAsync(ProcessPaymentRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(processPaymentPath, request);

        if (!response.IsSuccessStatusCode)
        {
            throw new ProviderException($"Status code: {response.StatusCode}");
        }

        var result = await response.Content.ReadFromJsonAsync<ProcessPaymentResponse>();

        return result;
    }
}
