using PaymentGateway.Api.Models.Repository;

namespace PaymentGateway.Api.Services;

/// <summary>
/// Test repository for storing payment data.
/// </summary>
public interface IPaymentsRepository
{
    void Add(PaymentData payment);

    PaymentData Get(Guid id);
}