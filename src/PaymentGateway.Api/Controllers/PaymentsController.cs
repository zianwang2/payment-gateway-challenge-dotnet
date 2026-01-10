using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Exceptions;
using PaymentGateway.Api.Models.Mapping;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly PaymentsRepository _paymentsRepository;
    private readonly IBankServiceClient _bankServiceClient;

    public PaymentsController(PaymentsRepository paymentsRepository, IBankServiceClient bankServiceClient)
    {
        _paymentsRepository = paymentsRepository;
        _bankServiceClient = bankServiceClient;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetPaymentResponse>> GetPaymentAsync(Guid id)
    {
        var payment = _paymentsRepository.Get(id);

        if (payment == null)
        {
            return NotFound();
        }
        return Ok(payment.ToGetPaymentResponseModel());
    }

    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse>> PostPaymentAsync([FromBody] PostPaymentRequest request)
    {
        try
        {
            var result = await _bankServiceClient.ProcessPaymentAsync(request.ToProcessPaymentRequestModel());
            var paymentData = result.ToPaymentDataModel(request);
            _paymentsRepository.Add(paymentData);
            var payment = paymentData.ToPostPaymentResponseModel();
            return Ok(payment);
        }
        catch (ProviderException ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { ErrorMessage = ex.Message });
        }

    }
}