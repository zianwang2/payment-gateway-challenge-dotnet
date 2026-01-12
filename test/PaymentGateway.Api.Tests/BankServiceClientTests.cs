using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Logging;

using Moq;
using Moq.Protected;

using PaymentGateway.Api.Models.Exceptions;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests;

public class BankServiceClientTests
{

    [Fact]
    public async Task ProcessPaymentAsync_ReturnsResult_WhenProviderReturnsSuccess()
    {
        // Arrange
        ProcessPaymentResponse providerResponse = new ProcessPaymentResponse
        {
            Authorized = true,
            AuthorizationCode = "123",
        };

        string json = JsonSerializer.Serialize(providerResponse);
        HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        Mock<HttpMessageHandler> handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(httpResponse);

        HttpClient httpClient = CreateHttpClientFromHandler(handlerMock.Object);
        Mock<ILogger<BankServiceClient>> loggerMock = new Mock<ILogger<BankServiceClient>>();
        BankServiceClient bankServiceClient = new BankServiceClient(httpClient, loggerMock.Object);

        ProcessPaymentRequest request = new ProcessPaymentRequest
        {
            CardNumber = "4111111111111111",
            ExpiryDate = "12/2026",
            Cvv = "123",
            Amount = 100,
            Currency = "GBP"
        };

        // Act
        ProcessPaymentResponse result = await bankServiceClient.ProcessPaymentAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Authorized);
        Assert.Equal("123", result.AuthorizationCode);
    }

    [Fact]
    public async Task ProcessPaymentAsync_ReturnsResult_WhenProviderReturnsFails()
    {
        // Arrange
        ProcessPaymentResponse providerResponse = new ProcessPaymentResponse
        {
            Authorized = false,
            AuthorizationCode = "",
        };

        string json = JsonSerializer.Serialize(providerResponse);
        HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        Mock<HttpMessageHandler> handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(httpResponse);

        HttpClient httpClient = CreateHttpClientFromHandler(handlerMock.Object);
        Mock<ILogger<BankServiceClient>> loggerMock = new Mock<ILogger<BankServiceClient>>();
        BankServiceClient bankServiceClient = new BankServiceClient(httpClient, loggerMock.Object);

        ProcessPaymentRequest request = new ProcessPaymentRequest
        {
            CardNumber = "4111111111111111",
            ExpiryDate = "12/2026",
            Cvv = "123",
            Amount = 100,
            Currency = "GBP"
        };

        // Act
        ProcessPaymentResponse result = await bankServiceClient.ProcessPaymentAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Authorized);
        Assert.Equal("", result.AuthorizationCode);
    }

    [Fact]
    public async Task ProcessPaymentAsync_ReturnBadRequest()
    {
        // Arrange
        ProcessPaymentResponse providerResponse = new ProcessPaymentResponse
        {
        };

        string json = JsonSerializer.Serialize(providerResponse);
        HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        Mock<HttpMessageHandler> handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(httpResponse);

        HttpClient httpClient = CreateHttpClientFromHandler(handlerMock.Object);
        Mock<ILogger<BankServiceClient>> loggerMock = new Mock<ILogger<BankServiceClient>>();
        BankServiceClient bankServiceClient = new BankServiceClient(httpClient, loggerMock.Object);

        ProcessPaymentRequest request = new ProcessPaymentRequest
        {
            CardNumber = "4111111111111111",
            Cvv = "123",
            Amount = 100,
            Currency = "GBP"
        };

        // Act
        try
        {
            ProcessPaymentResponse result = await bankServiceClient.ProcessPaymentAsync(request);
        }
        catch (ProviderException ex)
        {
            // Assert
            Assert.Equal("Provider returned non-success status. Status code: BadRequest", ex.Message);
        }
    }

    private HttpClient CreateHttpClientFromHandler(HttpMessageHandler handler)
    {
        HttpClient client = new HttpClient(handler) { BaseAddress = new Uri("http://test") };
        return client;
    }


}