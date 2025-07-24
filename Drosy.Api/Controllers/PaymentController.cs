using Azure.Core;
using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Payments.DTOs;
using Drosy.Application.UseCases.Payments.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
    : ControllerBase
{
    private readonly IPaymentService _paymentService = paymentService;
    private readonly ILogger<PaymentController> _logger = logger;

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