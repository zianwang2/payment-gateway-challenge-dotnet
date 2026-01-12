using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Exceptions;
using PaymentGateway.Api.Models.Mapping;
using PaymentGateway.Api.Models.Repository;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly IBankServiceClient _bankServiceClient;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentsRepository paymentsRepository, IBankServiceClient bankServiceClient, ILogger<PaymentsController> logger)
    {
        _paymentsRepository = paymentsRepository;
        _bankServiceClient = bankServiceClient;
        _logger = logger;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetPaymentResponse>> GetPaymentAsync(Guid id)
    {
        PaymentData payment = _paymentsRepository.Get(id);

        if (payment == null)
        {
            _logger.LogInformation("Payment not found. RequestId={RequestId} PaymentId={PaymentId}", HttpContext.TraceIdentifier, id);
            return NotFound(new {ErrorMessage = $"Payment not found. PaymentId={id}" });
        }

        _logger.LogInformation("Payment found. RequestId={RequestId} PaymentId={PaymentId} Status={Status}", HttpContext.TraceIdentifier, id, payment.Status);
        return Ok(payment.ToGetPaymentResponseModel());
    }

    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse>> PostPaymentAsync([FromBody] PostPaymentRequest request)
    {
        string requestId = HttpContext.TraceIdentifier;
        _logger.LogInformation("PostPayment received. RequestId={RequestId} CardNumberLastFour={CardNumberLastFour}", requestId, request.CardNumber[^4..]);

        try
        {
            // Process payment with bank service
            _logger.LogInformation("Payment starts to process. RequestId={RequestId} CardNumberLastFour={CardNumberLastFour}", requestId, request.CardNumber[^4..]);
            ProcessPaymentResponse result = await _bankServiceClient.ProcessPaymentAsync(request.ToProcessPaymentRequestModel());
            _logger.LogInformation("Payment processed. RequestId={RequestId} CardNumberLastFour={CardNumberLastFour}", requestId, request.CardNumber[^4..]);
            
            // Save payment data
            PaymentData paymentData = result.ToPaymentDataModel(request);
            _logger.LogInformation("Saving payment data. RequestId={RequestId} PaymentId={PaymentId} Status={Status} CardNumberLastFour={CardNumberLastFour}", requestId, paymentData.Id, paymentData.Status, paymentData.CardNumberLastFour);
            _paymentsRepository.Add(paymentData);
            _logger.LogInformation("Payment data saved. RequestId={RequestId} PaymentId={PaymentId} Status={Status} CardNumberLastFour={CardNumberLastFour}", requestId, paymentData.Id, paymentData.Status, paymentData.CardNumberLastFour);

            var payment = paymentData.ToPostPaymentResponseModel();
            return Ok(payment);
        }
        catch (ProviderException ex)
        {
            _logger.LogError(ex, "Provider error while processing payment. RequestId={RequestId}", requestId);
            return StatusCode(StatusCodes.Status502BadGateway, new { ErrorMessage = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while processing payment. RequestId={RequestId}", requestId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = "An unexpected error occurred" });
        }
    }
}