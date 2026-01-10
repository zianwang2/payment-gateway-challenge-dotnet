using PaymentGateway.Api.Models.Repository;
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
    }
}
