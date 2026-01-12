using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Mapping;
using PaymentGateway.Api.Models.Repository;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests;

public class PaymentExtentionsTests
{
    [Fact]
    public void ToGetPaymentResponseModel_MapsAllFields()
    {
        var data = new PaymentData
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Authorized,
            CardNumberLastFour = "1234",
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Currency = "USD",
            Amount = 500
        };

        var result = data.ToGetPaymentResponseModel();

        Assert.Equal(data.Id, result.Id);
        Assert.Equal(data.Status, result.Status);
        Assert.Equal(data.CardNumberLastFour, result.CardNumberLastFour);
        Assert.Equal(data.ExpiryMonth, result.ExpiryMonth);
        Assert.Equal(data.ExpiryYear, result.ExpiryYear);
        Assert.Equal(data.Currency, result.Currency);
        Assert.Equal(data.Amount, result.Amount);
    }

    [Fact]
    public void ToPostPaymentResponseModel_MapsAllFields()
    {
        var data = new PaymentData
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Declined,
            CardNumberLastFour = "9876",
            ExpiryMonth = 6,
            ExpiryYear = 2026,
            Currency = "GBP",
            Amount = 1000
        };

        var result = data.ToPostPaymentResponseModel();

        Assert.Equal(data.Id, result.Id);
        Assert.Equal(data.Status, result.Status);
        Assert.Equal(data.CardNumberLastFour, result.CardNumberLastFour);
        Assert.Equal(data.ExpiryMonth, result.ExpiryMonth);
        Assert.Equal(data.ExpiryYear, result.ExpiryYear);
        Assert.Equal(data.Currency, result.Currency);
        Assert.Equal(data.Amount, result.Amount);
    }

    [Fact]
    public void ToProcessPaymentRequestModel_FormatsExpiryDateAndMapsFields()
    {
        var post = new PostPaymentRequest
        {
            CardNumber = "4111111111111111",
            ExpiryMonth = 3,
            ExpiryYear = 2029,
            Currency = "usd",
            Amount = 250,
            Cvv = "123"
        };

        var result = post.ToProcessPaymentRequestModel();

        Assert.Equal(post.CardNumber, result.CardNumber);
        Assert.Equal("03/2029", result.ExpiryDate);
        Assert.Equal(post.Currency, result.Currency);
        Assert.Equal(post.Amount, result.Amount);
        Assert.Equal(post.Cvv, result.Cvv);
    }

    [Fact]
    public void ToPaymentDataModel_SetsFieldsFromProcessResponseAndPostRequest()
    {
        var post = new PostPaymentRequest
        {
            CardNumber = "5555444433332222",
            ExpiryMonth = 11,
            ExpiryYear = 2027,
            Currency = "gbp",
            Amount = 750,
            Cvv = "321"
        };

        var processResponse = new ProcessPaymentResponse
        {
            Authorized = true,
            AuthorizationCode = "123"
        };

        var result = processResponse.ToPaymentDataModel(post);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(PaymentStatus.Authorized, result.Status);
        Assert.Equal("2222", result.CardNumberLastFour);
        Assert.Equal(post.ExpiryMonth, result.ExpiryMonth);
        Assert.Equal(post.ExpiryYear, result.ExpiryYear);
        Assert.Equal(post.Currency.ToUpper(), result.Currency);
        Assert.Equal(post.Amount, result.Amount);
    }

    [Fact]
    public void ToPaymentDataModel_WhenDeclined_SetsDeclinedStatus()
    {
        var post = new PostPaymentRequest
        {
            CardNumber = "5555444433332222",
            ExpiryMonth = 11,
            ExpiryYear = 2027,
            Currency = "gbp",
            Amount = 750,
            Cvv = "321"
        };

        var processResponse = new ProcessPaymentResponse
        {
            Authorized = false,
            AuthorizationCode = ""
        };

        var result = processResponse.ToPaymentDataModel(post);

        Assert.Equal(PaymentStatus.Declined, result.Status);
    }
}
