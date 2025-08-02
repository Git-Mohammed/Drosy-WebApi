using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Payments.Services;
using Drosy.Application.UseCases.Payments.Interfaces;
using Drosy.Application.UseCases.Payments.DTOs;
using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Students.Interfaces;
using Drosy.Application.UseCases.Plans.Interfaces;
using Drosy.Application.UseCases.PlanStudents.Interfaces;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ErrorComponents.EFCore;
using Drosy.Domain.Shared.ErrorComponents.Payments;

namespace Drosy.Tests.Application.Payments;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock = new();
    private readonly Mock<ILogger<PaymentService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IStudentService> _studentServiceMock = new();
    private readonly Mock<IPlanService> _planServiceMock = new();
    private readonly Mock<IPlanStudentsService> _planStudentsServiceMock = new();

    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _paymentService = new PaymentService(
            _paymentRepositoryMock.Object,
            _loggerMock.Object,
            _mapperMock.Object,
            _unitOfWorkMock.Object,
            _studentServiceMock.Object,
            _planServiceMock.Object,
            _planStudentsServiceMock.Object
        );
    }

    [Fact]
    public async Task CreatePaymentAsync_Should_ReturnSuccess_WhenEverythingIsValid()
    {
        // Arrange
        var createPaymentDto = new CreatePaymentDto
        {
            StudentId = 1,
            PlanId = 2,
            Amount = 300,
        };

        var paymentEntity = new Payment();
        var paymentDto = new PaymentDto();

        _studentServiceMock.Setup(x => x.ExistsAsync(createPaymentDto.StudentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        _planServiceMock.Setup(x => x.GetPlanByIdAsync(createPaymentDto.PlanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new PlanDto()));
        
        _planStudentsServiceMock.Setup(x =>
                x.IsStudentInPlanAsync(createPaymentDto.PlanId, createPaymentDto.StudentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        _mapperMock.Setup(x => x.Map<CreatePaymentDto, Payment>(createPaymentDto))
            .Returns(paymentEntity);

        _paymentRepositoryMock.Setup(x => x.AddAsync(paymentEntity, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mapperMock.Setup(x => x.Map<Payment, PaymentDto>(paymentEntity))
            .Returns(paymentDto);

        // Act
        var result = await _paymentService.CreateAsync(createPaymentDto, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(paymentDto, result.Value);
    }

    [Fact]
    public async Task CreatePaymentAsync_Should_ReturnFailure_WhenStudentNotFound()
    {
        // Arrange
        var createPaymentDto = new CreatePaymentDto
        {
            StudentId = 100,
            PlanId = 2,
            Amount = 300,
        };

        _studentServiceMock.Setup(x => x.ExistsAsync(createPaymentDto.StudentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(CommonErrors.NotFound));
        
        // Act
        var result = await _paymentService.CreateAsync(createPaymentDto, CancellationToken.None);
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(CommonErrors.NotFound, result.Error);
        
        _loggerMock.Verify(x => x.LogError($"Student with id {createPaymentDto.StudentId} not found"), Times.Once);
        _planServiceMock.Verify(x => x.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _paymentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task CreatePaymentAsync_Should_ReturnFailure_WhenPlanNotFound()
    {
        // Arrange
        var createPaymentDto = new CreatePaymentDto
        {
            StudentId = 1,
            PlanId = 100,
            Amount = 300,
        };

        _studentServiceMock.Setup(x => x.ExistsAsync(createPaymentDto.StudentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        _planServiceMock.Setup(x => x.GetPlanByIdAsync(createPaymentDto.PlanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<PlanDto>(CommonErrors.NotFound));
        
        // Act
        var result = await _paymentService.CreateAsync(createPaymentDto, CancellationToken.None);
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(CommonErrors.NotFound, result.Error);
        _loggerMock.Verify(x => x.LogError($"Plan with id {createPaymentDto.PlanId} not found"), Times.Once);
        _mapperMock.Verify(x => x.Map<CreatePaymentDto, Payment>(It.IsAny<CreatePaymentDto>()), Times.Never);
        _paymentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task CreatePaymentAsync_Should_ReturnFailure_WhenSaveChangesFails()
    {
        // Arrange
        var createPaymentDto = new CreatePaymentDto
        {
            StudentId = 1,
            PlanId = 100,
            Amount = 300,
        };
        
        var paymentEntity = new Payment();
        var paymentDto = new PaymentDto();
        
        _studentServiceMock.Setup(x => x.ExistsAsync(createPaymentDto.StudentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        _planServiceMock.Setup(x => x.GetPlanByIdAsync(createPaymentDto.PlanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new PlanDto()));
        
        _planStudentsServiceMock.Setup(x =>
                x.IsStudentInPlanAsync(createPaymentDto.PlanId, createPaymentDto.StudentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));
        
        _mapperMock.Setup(x => x.Map<CreatePaymentDto, Payment>(createPaymentDto))
            .Returns(paymentEntity);
        
        _paymentRepositoryMock.Setup(x => x.AddAsync(paymentEntity, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        
        // Act
        var result = await _paymentService.CreateAsync(createPaymentDto, CancellationToken.None);
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(EfCoreErrors.CanNotSaveChanges, result.Error);
        _loggerMock.Verify(x => x.LogError("Error creating plan", It.IsAny<object[]>()), Times.Once);
        _mapperMock.Verify(x => x.Map<Payment, PaymentDto>(It.IsAny<Payment>()), Times.Never);
    }
    
    [Fact]
    public async Task CreatePaymentAsync_Should_ReturnFailure_WhenOperationCancelled()
    {
        // Arrange
        var createPaymentDto = new CreatePaymentDto
        {
            StudentId = 1,
            PlanId = 2,
            Amount = 300,
        };

        // throw: OperationCanceledException
        _studentServiceMock.Setup(x => x.ExistsAsync(createPaymentDto.StudentId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException("Operation was cancelled"));

        // Act
        var result = await _paymentService.CreateAsync(createPaymentDto, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(CommonErrors.OperationCancelled, result.Error);

        _loggerMock.Verify(x => x.LogError("Operation was cancelled"), Times.Once);

        // verify not executed 
        _planServiceMock.Verify(x => x.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _paymentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mapperMock.Verify(x => x.Map<Payment, PaymentDto>(It.IsAny<Payment>()), Times.Never);
    }

    [Fact]
    public async Task CreatePaymentAsync_Should_ReturnFailure_WhenStudentNotAssignToPlan()
    {
        // arrange
        var createPayment = new CreatePaymentDto
        {
            PlanId = 1,
            StudentId = 3,
            Amount = 300,
        };
        
        _studentServiceMock.Setup(x => x.ExistsAsync(createPayment.StudentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));
        _planServiceMock.Setup(x => x.GetPlanByIdAsync(createPayment.PlanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new PlanDto()));
        _planStudentsServiceMock.Setup(x =>
                x.IsStudentInPlanAsync(createPayment.PlanId, createPayment.StudentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(CommonErrors.NotFound));
        
        var result = await _paymentService.CreateAsync(createPayment, CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(CommonErrors.NotFound, result.Error);
        _loggerMock.Verify(x => x.LogError($"Student with id: {createPayment.StudentId} not assign to this plan", It.IsAny<object[]>()), Times.Once);
        _mapperMock.Verify(x => x.Map<Payment, PaymentDto>(It.IsAny<Payment>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mapperMock.Verify(x => x.Map<Payment, PaymentDto>(It.IsAny<Payment>()), Times.Never);
        _paymentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePaymentAsync_Should_ReturnFailure_WhenPaymentNotExists()
    {
        // arrange
        var paymentId = 1;
        var updatePaymentDto = new UpdatePaymentDto
        {
            StudentId = 1,
            PlanId = 2,
            Amount = 300,
            Method = PaymentMethod.Cash
        };
        
        _paymentRepositoryMock.Setup(x=> x.GetByIdAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Payment);
        
        var result = await _paymentService.UpdateAsync(paymentId, updatePaymentDto, CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(PaymentErrors.PaymentNotFound, result.Error);
    }

}