using Drosy.Application.UseCases.Payments.DTOs;
using Drosy.Domain.Shared.ApplicationResults;

namespace Drosy.Application.UseCases.Payments.Interfaces;

public interface IPaymentService
{
    Task<Result<PaymentDto>> CreatePaymentAsync(CreatePaymentDto paymentDto, CancellationToken  cancellation);
}