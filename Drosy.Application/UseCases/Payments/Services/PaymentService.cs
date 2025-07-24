using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Payments.DTOs;
using Drosy.Application.UseCases.Payments.Interfaces;
using Drosy.Application.UseCases.Plans.Interfaces;
using Drosy.Application.UseCases.Students.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ErrorComponents.EFCore;

namespace Drosy.Application.UseCases.Payments.Services;

public class PaymentService(
    IPaymentRepository paymentRepository,
    ILogger<PaymentService> logger,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    IStudentService  studentService,
    IPlanService   planService)
    : IPaymentService
{
    private readonly IStudentService _studentService  = studentService;
    private readonly IPlanService _planService  = planService;
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly ILogger<PaymentService> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<PaymentDto>> CreatePaymentAsync(CreatePaymentDto paymentDto, CancellationToken cancellation)
    {
        try
        {
            var isStudentExisting = await _studentService.ExistsAsync(paymentDto.StudentId, cancellation);
            if (!isStudentExisting.IsSuccess)
            {
                _logger.LogError($"Student with id {paymentDto.StudentId} not found");
                return Result.Failure<PaymentDto>(isStudentExisting.Error);
            }
            
            var isPlanExisting = await _planService.ExistsAsync(paymentDto.PlanId, cancellation);
            if (!isPlanExisting.IsSuccess)
            {
                _logger.LogError($"Plan with id {paymentDto.PlanId} not found");
                return Result.Failure<PaymentDto>(isPlanExisting.Error);
            }
            
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
}