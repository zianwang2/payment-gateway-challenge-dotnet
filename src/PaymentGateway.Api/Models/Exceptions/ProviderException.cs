namespace PaymentGateway.Api.Models.Exceptions;

public class ProviderException : Exception
{
    public ProviderException(string message) : base(message)
    {
    }
}
