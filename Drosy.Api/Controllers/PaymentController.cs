using Azure.Core;
using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Payments.DTOs;
using Drosy.Application.UseCases.Payments.Interfaces;
using Drosy.Domain.Shared.ErrorComponents;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Controllers;

/// <summary>
/// Exposes endpoints to manage student payments, including creation and history retrieval.
/// </summary>
[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger) : ControllerBase
{
    private readonly IPaymentService _paymentService = paymentService;
    private readonly ILogger<PaymentController> _logger = logger;

    #region Read

    /// <summary>
    /// Retrieves a payment by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the payment to retrieve.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <returns>
    /// 200 OK with the payment data.  
    /// 404 Not Found if no payment exists with the given ID.  
    /// 500 Internal Server Error for unexpected issues.
    /// </returns>
    [HttpGet]
    [Route("{id}", Name = "GetPaymentByIdAsync")]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPaymentByIdAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _paymentService.GetByIdAsync(id, cancellationToken);
        const string operation = nameof(GetPaymentByIdAsync);

        if (result.IsSuccess)
            return ApiResponseFactory.SuccessResponse(result.Value);

        _logger.LogError("Error retrieving payment [{id}]: {Error}", id, result.Error);
        return ApiResponseFactory.FromFailure(result, operation, nameof(PaymentDto));
    }

    /// <summary>
    /// Retrieves a student’s payment history, optionally filtered by date, method, or other criteria.
    /// </summary>
    /// <param name="studentId">The ID of the student to fetch history for.</param>
    /// <param name="filter">Optional filtering parameters for narrowing the result.</param>
    /// <param name="cancellation">Token used to cancel the request if needed.</param>
    /// <returns>
    /// 200 OK with payment history results.  
    /// 404 Not Found if the student does not exist or has no history.  
    /// 422 Unprocessable Entity for invalid filters.  
    /// 500 Internal Server Error for unexpected issues.
    /// </returns>
    [HttpGet("students/{studentId}/payment-history", Name = "GetStudentPaymentHistoryAsync")]
    [ProducesResponseType(typeof(ApiResponse<StudentPaymentHistoryDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStudentPaymentHistoryAsync(
        int studentId,
        [FromQuery] PaymentHistoryFilterDTO filter,
        CancellationToken cancellation)
    {
        if (filter == null)
        {
            var error = new ApiError("filter", ErrorMessageResourceRepository.GetMessage(CommonErrors.NullValue.Message, AppError.CurrentLanguage));
            return ApiResponseFactory.BadRequestResponse("filter", "Invalid filter parameters.", error.Message);
        }

        try
        {
            var result = await _paymentService.GetStudentPaymentHistoryAsync(studentId, filter, cancellation);

            if (result.IsFailure)
            {
                return ApiResponseFactory.FromFailure(result, nameof(GetStudentPaymentHistoryAsync), "PaymentHistory");
            }

            return ApiResponseFactory.SuccessResponse(result.Value, "Student payment history retrieved successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponseFactory.FromException(ex);
        }
    }

    #endregion

    #region Write

    /// <summary>
    /// Creates and stores a new payment record for a student.
    /// </summary>
    /// <param name="payment">DTO containing the payment details to store.</param>
    /// <param name="ct">Token used to cancel the request if needed.</param>
    /// <returns>
    /// 201 Created with the newly saved payment data.  
    /// 422 Unprocessable Entity if validation fails.  
    /// 500 Internal Server Error for unexpected issues.
    /// </returns>
    [HttpPost(Name = "CrateNewPaymentAsync")]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CrateNewPaymentAsync([FromBody] CreatePaymentDto payment, CancellationToken ct)
    {
        var result = await _paymentService.CreateAsync(payment, ct);

        if (result.IsSuccess)
        {
            return ApiResponseFactory.CreatedResponse("GetPaymentByIdAsync", new { id = result.Value.Id }, result.Value);
        }

        _logger.LogError(result.Error.Message);
        return ApiResponseFactory.FromFailure(result, "CreatePaymentAsync", "CreatePayment");
    }

    [HttpPut("{id}",Name = "UpdatePaymentAsync")]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdatePaymentAsync(int id,UpdatePaymentDto payment, CancellationToken ct)
    {
        var result = await _paymentService.UpdateAsync(id,payment, ct);
        if (result.IsSuccess)
        {
            return ApiResponseFactory.SuccessResponse();
        }
        _logger.LogError(result.Error.Message);
        return ApiResponseFactory.FromFailure(result, "UpdatePaymentAsync", "UpdatePayment");
    }

    #endregion
}
