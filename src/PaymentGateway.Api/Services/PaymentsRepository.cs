using PaymentGateway.Api.Models.Repository;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Models.Mapping;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository
{
    public List<PaymentData> Payments = new();
    
    public void Add(PaymentData payment)
    {
        Payments.Add(payment);
    }

    public PaymentData Get(Guid id)
    {
        return Payments.FirstOrDefault(p => p.Id == id);
    }
}