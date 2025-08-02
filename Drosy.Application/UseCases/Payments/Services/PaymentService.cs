using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Payments.DTOs;
using Drosy.Application.UseCases.Payments.Interfaces;
using Drosy.Application.UseCases.Plans.Interfaces;
using Drosy.Application.UseCases.PlanStudents.Interfaces;
using Drosy.Application.UseCases.Students.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ErrorComponents.EFCore;
using Drosy.Domain.Shared.ErrorComponents.Payments;

namespace Drosy.Application.UseCases.Payments.Services;

public class PaymentService(
    IPaymentRepository paymentRepository,
    ILogger<PaymentService> logger,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    IStudentService  studentService,
    IPlanService   planService,
    IPlanStudentsService   planStudentsService)
    : IPaymentService
{
    private readonly IStudentService _studentService  = studentService;
    private readonly IPlanService _planService  = planService;
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly ILogger<PaymentService> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPlanStudentsService _planStudentsService = planStudentsService;

    public async Task<Result<PaymentDto>> CreateAsync(CreatePaymentDto paymentDto, CancellationToken cancellation)
    {
        try
        {
            var isStudentExisting = await _studentService.ExistsAsync(paymentDto.StudentId, cancellation);
            if (!isStudentExisting.IsSuccess)
            {
                _logger.LogError($"Student with id {paymentDto.StudentId} not found");
                return Result.Failure<PaymentDto>(isStudentExisting.Error);
            }
            
            var existingPlan = await _planService.GetPlanByIdAsync(paymentDto.PlanId, cancellation);
            if (!existingPlan.IsSuccess)
            {
                _logger.LogError($"Plan with id {paymentDto.PlanId} not found");
                return Result.Failure<PaymentDto>(existingPlan.Error);
            }
            
            // TODO: check if student assign to plan (used planStudentService)
            
            var isStudentAssignToPlan = await _planStudentsService.IsStudentInPlanAsync(paymentDto.PlanId, paymentDto.StudentId, cancellation);
            if (!isStudentAssignToPlan.IsSuccess)
            {
                _logger.LogError($"Student with id: {paymentDto.StudentId} not assign to this plan");
                return Result.Failure<PaymentDto>(isStudentAssignToPlan.Error);
            }
            // TODO: check amount is in range
            
            var createPayment = _mapper.Map<CreatePaymentDto, Payment>(paymentDto);
            await _paymentRepository.AddAsync(createPayment, cancellation);
            var result = await _unitOfWork.SaveChangesAsync(cancellation);
            if (!result)
            {
                _logger.LogError("Error creating plan", paymentDto);
                return Result.Failure<PaymentDto>(EfCoreErrors.CanNotSaveChanges);
            }
            var payment = _mapper.Map<Payment, PaymentDto>(createPayment);
            return Result.Success(payment);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex.Message);
            return Result.Failure<PaymentDto>(CommonErrors.OperationCancelled);
        }
    }


    public async Task<Result<PaymentDto>> GetByIdAsync(int id, CancellationToken cancellation)
    {
        var paymentExisting = await _paymentRepository.GetByIdAsync(id, cancellation);
        if (paymentExisting is null)
        {
            _logger.LogError($"Payment with id {id} not found");
            return Result.Failure<PaymentDto>(CommonErrors.NotFound);
        }
        var payment = _mapper.Map<Payment, PaymentDto>(paymentExisting);
        return Result.Success(payment);
    }
    public async Task<Result<StudentPaymentHistoryDTO>> GetStudentPaymentHistoryAsync(
        int studentId,
        PaymentHistoryFilterDTO filter,
        CancellationToken cancellation)
    {
        try
        {
            var exists = await _studentService.ExistsAsync(studentId, cancellation);
            if (!exists.IsSuccess)
                return Result.Failure<StudentPaymentHistoryDTO>(exists.Error);

            var payments = await _paymentRepository
                .GetStudentPaymentsAsync(studentId, filter.FromDate, filter.ToDate, cancellation);

            if (payments == null || payments.Count == 0)
                return Result.Failure<StudentPaymentHistoryDTO>(PaymentErrors.PaymentNotFound);

            var totalFee = await _paymentRepository.GetStudentTotalFeeAsync(studentId, cancellation);
            if (totalFee < 0)
                return Result.Failure<StudentPaymentHistoryDTO>(PaymentErrors.AmountMustBePositive);

            var totalPaid = payments.Sum(p => p.Amount);
            var remaining = totalFee - totalPaid;
            if (remaining < 0)
                return Result.Failure<StudentPaymentHistoryDTO>(PaymentErrors.PaymentExceedsOutstandingBalance);

            var dto = new StudentPaymentHistoryDTO
            {
                StudentId = studentId,
                TotalPaid = totalPaid,
                RemainingBalance = remaining,
                Payments = _mapper.Map<List<Payment>, List<PaymentDetailDTO>>(payments)
            };

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Failed to retrieve payment history for student {StudentId}", studentId);
            return Result.Failure<StudentPaymentHistoryDTO>(PaymentErrors.PaymentSaveFailure); // More specific than CommonErrors.Unexpected
        }
    }
    
    public async Task<Result> UpdateAsync(int id,UpdatePaymentDto paymentDto, CancellationToken cancellation)
    {
        var existingPayment = await _paymentRepository.GetByIdAsync(id, cancellation);
        if (existingPayment is null)
        {
            _logger.LogError($"Payment with id {id} not found");
            return Result.Failure(PaymentErrors.PaymentNotFound);
        }
        
        existingPayment.Method = paymentDto.Method;
        existingPayment.PlanId = paymentDto.PlanId;
        existingPayment.StudentId = paymentDto.StudentId;
        existingPayment.Amount = paymentDto.Amount;
        existingPayment.Notes = string.IsNullOrWhiteSpace(paymentDto.Notes) ? existingPayment.Notes : paymentDto.Notes;
        
        await _paymentRepository.UpdateAsync(existingPayment, cancellation);
        var result = await _unitOfWork.SaveChangesAsync(cancellation);
        if (!result)
        {
            _logger.LogError("Failed to update payment", paymentDto);
            return Result.Failure(EfCoreErrors.CanNotSaveChanges);
        }
        return Result.Success();
    }


}