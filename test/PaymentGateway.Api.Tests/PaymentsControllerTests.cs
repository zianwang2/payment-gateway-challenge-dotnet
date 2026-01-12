using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Exceptions;
using PaymentGateway.Api.Models.Repository;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests
{
    private readonly Random _random = new();
    Mock<IBankServiceClient> mockProvider = new Mock<IBankServiceClient>();

    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new PaymentData
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999).ToString(),
            Currency = "GBP"
        };

        var paymentsRepository = new PaymentsRepository();
        paymentsRepository.Add(payment);

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton(paymentsRepository)))
            .CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ReturnsBadRequestForPostPayment()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", new { });
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReturnsBadRequestForInvalidCardNumberForPostPayment()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();

        var postPaymentRequest = new
        {
            CardNumber = "1234",
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Cvv = "123",
            Amount = 100,
            Currency = "usd"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", postPaymentRequest);
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReturnsBadRequestForInvalidCardNumberTooLongForPostPayment()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();

        var postPaymentRequest = new
        {
            CardNumber = "12345678901234567890",
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Cvv = "123",
            Amount = 100,
            Currency = "usd"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", postPaymentRequest);
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReturnsBadRequestForInvalidDateForPostPayment()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();

        var postPaymentRequest = new
        {
            CardNumber = "1234567890123456",
            ExpiryMonth = 12,
            ExpiryYear = 2020,
            Cvv = "123",
            Amount = 100,
            Currency = "usd"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", postPaymentRequest);
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReturnsBadRequestForInvalidDate2ForPostPayment()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();

        var postPaymentRequest = new
        {
            CardNumber = "1234567890123456",
            ExpiryMonth = 15,
            ExpiryYear = 2027,
            Cvv = "123",
            Amount = 100,
            Currency = "usd"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", postPaymentRequest);
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReturnsBadRequestForInvalidDate3ForPostPayment()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();

        var postPaymentRequest = new
        {
            CardNumber = "1234567890123456",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            Cvv = "123",
            Amount = 100,
            Currency = "usd"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", postPaymentRequest);
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReturnsBadRequestForInvalidCvvForPostPayment()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();

        var postPaymentRequest = new
        {
            CardNumber = "1234567890123456",
            ExpiryMonth = 12,
            ExpiryYear = 2026,
            Cvv = "ab1",
            Amount = 100,
            Currency = "usd"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", postPaymentRequest);
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReturnsBadRequestForInvalidCvv2ForPostPayment()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();

        var postPaymentRequest = new
        {
            CardNumber = "1234567890123456",
            ExpiryMonth = 12,
            ExpiryYear = 2026,
            Cvv = "12",
            Amount = 100,
            Currency = "usd"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", postPaymentRequest);
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReturnsBadRequestForInvalidCvv3ForPostPayment()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();

        var postPaymentRequest = new
        {
            CardNumber = "1234567890123456",
            ExpiryMonth = 12,
            ExpiryYear = 2026,
            Cvv = "123456",
            Amount = 100,
            Currency = "usd"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", postPaymentRequest);
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReturnsBadRequestForInvalidAmountForPostPayment()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();

        var postPaymentRequest = new
        {
            CardNumber = "1234567890123456",
            ExpiryMonth = 12,
            ExpiryYear = 2026,
            Cvv = "123",
            Amount = -1,
            Currency = "usd"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", postPaymentRequest);
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReturnsBadRequestForInvalidCurrencyForPostPayment()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();

        var postPaymentRequest = new
        {
            CardNumber = "1234567890123456",
            ExpiryMonth = 12,
            ExpiryYear = 2026,
            Cvv = "123",
            Amount = 100,
            Currency = "abc"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", postPaymentRequest);
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReturnsBadGatewayWhenProviderFails()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        mockProvider.Setup(v => v.ProcessPaymentAsync(It.IsAny<ProcessPaymentRequest>())).ThrowsAsync(new ProviderException("Provider failure"));
        var client = webApplicationFactory.WithWebHostBuilder(builder => builder.ConfigureServices(services => ((ServiceCollection)services)
            .AddSingleton<IBankServiceClient>(mockProvider.Object)))
            .CreateClient();
        
        var postPaymentRequest = new
        {
            CardNumber = "123456789123456",
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Cvv = "123",
            Amount = 100,
            Currency = "usd"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", postPaymentRequest);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
    }

    [Fact]
    public async Task ReturnsAuthorizedResponse()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        mockProvider.Setup(v => v.ProcessPaymentAsync(It.IsAny<ProcessPaymentRequest>())).ReturnsAsync(new ProcessPaymentResponse { Authorized = true, AuthorizationCode = Guid.NewGuid().ToString()});
        var client = webApplicationFactory.WithWebHostBuilder(builder => builder.ConfigureServices(services => ((ServiceCollection)services)
            .AddSingleton<IBankServiceClient>(mockProvider.Object)))
            .CreateClient();

        var postPaymentRequest = new
        {
            CardNumber = "123456789123456",
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Cvv = "123",
            Amount = 100,
            Currency = "usd"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", postPaymentRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response);
        // check the response content
        var responseContent = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        Assert.NotNull(responseContent);
        Assert.Equal(PaymentStatus.Authorized, responseContent.Status);
    }

    [Fact]
    public async Task ReturnsDeclinedResponse()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        mockProvider.Setup(v => v.ProcessPaymentAsync(It.IsAny<ProcessPaymentRequest>())).ReturnsAsync(new ProcessPaymentResponse { Authorized = false, AuthorizationCode = "" });
        var client = webApplicationFactory.WithWebHostBuilder(builder => builder.ConfigureServices(services => ((ServiceCollection)services)
            .AddSingleton<IBankServiceClient>(mockProvider.Object)))
            .CreateClient();

        var postPaymentRequest = new
        {
            CardNumber = "123456789123456",
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Cvv = "123",
            Amount = 100,
            Currency = "usd"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", postPaymentRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response);
        // check the response content
        var responseContent = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        Assert.NotNull(responseContent);
        Assert.Equal(PaymentStatus.Declined, responseContent.Status);
    }
}