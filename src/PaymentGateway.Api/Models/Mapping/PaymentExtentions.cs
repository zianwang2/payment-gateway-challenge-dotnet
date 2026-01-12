using PaymentGateway.Api.Models.Repository;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Models.Mapping
{
    public static class PaymentExtentions
    {
        public static GetPaymentResponse ToGetPaymentResponseModel(this PaymentData paymentData)
        {
            return new GetPaymentResponse
            {
                Id = paymentData.Id,
                Status = paymentData.Status,
                CardNumberLastFour = paymentData.CardNumberLastFour,
                ExpiryMonth = paymentData.ExpiryMonth,
                ExpiryYear = paymentData.ExpiryYear,
                Currency = paymentData.Currency,
                Amount = paymentData.Amount
            };
        }

        public static PostPaymentResponse ToPostPaymentResponseModel(this PaymentData paymentData)
        {
            return new PostPaymentResponse
            {
                Id = paymentData.Id,
                Status = paymentData.Status,
                CardNumberLastFour = paymentData.CardNumberLastFour,
                ExpiryMonth = paymentData.ExpiryMonth,
                ExpiryYear = paymentData.ExpiryYear,
                Currency = paymentData.Currency,
                Amount = paymentData.Amount
            };
        }

        public static ProcessPaymentRequest ToProcessPaymentRequestModel(this PostPaymentRequest postPaymentRequest)
        {
            return new ProcessPaymentRequest
            {
                CardNumber = postPaymentRequest.CardNumber,
                ExpiryDate = $"{postPaymentRequest.ExpiryMonth:D2}/{postPaymentRequest.ExpiryYear}",
                Currency = postPaymentRequest.Currency,
                Amount = postPaymentRequest.Amount,
                Cvv = postPaymentRequest.Cvv
            };
        }

        public static PaymentData ToPaymentDataModel(this ProcessPaymentResponse processPaymentResponse, PostPaymentRequest postPaymentRequest)
        {
            return new PaymentData
            {
                Id = Guid.NewGuid(),
                Status = processPaymentResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined,
                CardNumberLastFour = postPaymentRequest.CardNumber[^4..],
                ExpiryMonth = postPaymentRequest.ExpiryMonth,
                ExpiryYear = postPaymentRequest.ExpiryYear,
                Currency = postPaymentRequest.Currency,
                Amount = postPaymentRequest.Amount
            };
        }
    }
}

