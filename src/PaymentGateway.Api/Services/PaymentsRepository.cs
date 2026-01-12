using PaymentGateway.Api.Models.Repository;
namespace PaymentGateway.Api.Services;

/// <summary>
/// Test repository for storing payment data.
/// </summary>
public class PaymentsRepository: IPaymentsRepository
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