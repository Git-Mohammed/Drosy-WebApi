using Azure.Core;
using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Payments.DTOs;
using Drosy.Application.UseCases.Payments.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
    : ControllerBase
{
    private readonly IPaymentService _paymentService = paymentService;
    private readonly ILogger<PaymentController> _logger = logger;

    
    [HttpGet]
    [Route("{id}", Name = "GetPaymentByIdAsync")]
    public async Task<IActionResult> GetPaymentByIdAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _paymentService.GetByIdAsync(id, cancellationToken);
        const string operation = nameof(GetPaymentByIdAsync);
        if (result.IsSuccess) return ApiResponseFactory.SuccessResponse(result.Value);
        _logger.LogError("Error retrieving plan [{id}]: {Error}", id, result.Error);
        return ApiResponseFactory.FromFailure(result, operation, nameof(PaymentDto));
    }
    
    [HttpPost]
    public async Task<IActionResult> CrateNewPaymentAsync([FromBody] CreatePaymentDto payment, CancellationToken ct)
    {
        var result = await _paymentService.CreatePaymentAsync(payment, ct);
        if (result.IsSuccess)
            return ApiResponseFactory.CreatedResponse("GetPaymentByIdAsync", new{id = result.Value.Id},result.Value);
        _logger.LogError(result.Error.Message);
        return ApiResponseFactory.FromFailure(result, "CreatePaymentAsync","CreatePayment");
    }
    
}