using Drosy.Application.UseCases.PlanStudents.DTOs;
using Drosy.Application.UseCases.PlanStudents.Services;
using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Application.UseCases.Students.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ResultPattern;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents;
using Moq;
using Drosy.Application.Interfaces.Common;

namespace Drosy.Tests.Application.PlanStudents
{
    public class PlanStudentsServiceTests
    {
        private readonly Mock<IPlanStudentsRepository> _repoMock;
        private readonly Mock<IStudentService> _studentServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<PlanStudentsService>> _logger;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly PlanStudentsService _service;

        public PlanStudentsServiceTests()
        {
            _repoMock = new Mock<IPlanStudentsRepository>();
            _studentServiceMock = new Mock<IStudentService>();
            _mapperMock = new Mock<IMapper>();
            _uowMock = new Mock<IUnitOfWork>();
            _logger = new Mock<ILogger<PlanStudentsService>>();
            _service = new PlanStudentsService(
                _repoMock.Object,
                _studentServiceMock.Object,
                _logger.Object,
                _mapperMock.Object,
                _uowMock.Object);
        }

        #region Tests for AddStudentToPlanAsync
        [Fact]
        public async Task AddStudentToPlanAsync_ShouldReturnNotFound_WhenStudentNotExist()
        {
            // Arrange
            var dto = new AddStudentToPlanDto { StudentId = 1 };
            _studentServiceMock
                .Setup(s => s.GetByIdAsync(dto.StudentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<StudentDTO>(Error.NotFound, new Exception()));

            // Act
            var result = await _service.AddStudentToPlanAsync(10, dto, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(Error.NotFound, result.Error);
        }

        [Fact]
        public async Task AddStudentToPlanAsync_ShouldReturnConflict_WhenAlreadyInPlan()
        {
            // Arrange
            var dto = new AddStudentToPlanDto { StudentId = 2 };
            _studentServiceMock
                .Setup(s => s.GetByIdAsync(dto.StudentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(new StudentDTO()));
            _repoMock
                .Setup(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.AddStudentToPlanAsync(20, dto, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(Error.Conflict, result.Error);
        }

        [Fact]
        public async Task AddStudentToPlanAsync_ShouldReturnSuccess_WhenAllValid()
        {
            // Arrange
            var dto = new AddStudentToPlanDto { StudentId = 3 };
            var entity = new PlanStudent { StudentId = 3 };
            var dtoResult = new PlanStudentDto { StudentId = 3 };

            _studentServiceMock
                .Setup(s => s.GetByIdAsync(dto.StudentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(new StudentDTO()));
            _repoMock
                .Setup(r => r.ExistsAsync(It.IsAny<int>(),It.IsAny<int>() , It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _mapperMock
                .Setup(m => m.Map<AddStudentToPlanDto, PlanStudent>(dto))
                .Returns(entity);
            _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mapperMock
                .Setup(m => m.Map<PlanStudent, PlanStudentDto>(entity))
                .Returns(dtoResult);

            // Act
            var result = await _service.AddStudentToPlanAsync(30, dto, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dtoResult, result.Value);
            _repoMock.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddStudentToPlanAsync_ShouldReturnEFCoreFailure_WhenSaveFails()
        {
            // Arrange
            var dto = new AddStudentToPlanDto { StudentId = 4 };
            var entity = new PlanStudent { StudentId = 4 };

            _studentServiceMock
                .Setup(s => s.GetByIdAsync(dto.StudentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(new StudentDTO()));
            _repoMock
                .Setup(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _mapperMock
                .Setup(m => m.Map<AddStudentToPlanDto, PlanStudent>(dto))
                .Returns(entity);
            _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.AddStudentToPlanAsync(40, dto, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(Error.CanNotSaveChanges, result.Error);
        }

        [Fact]
        public async Task AddStudentToPlanAsync_ShouldReturnCancelled_WhenTokenCancelled()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            var result = await _service.AddStudentToPlanAsync(50, new AddStudentToPlanDto(), cts.Token);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(Error.OperationCancelled, result.Error);
        }

        [Fact]
        public async Task AddStudentToPlanAsync_ShouldReturnFailure_OnException()
        {
            // Arrange
            var dto = new AddStudentToPlanDto { StudentId = 5 };
            _studentServiceMock
                .Setup(s => s.GetByIdAsync(dto.StudentId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Oops"));

            // Act
            var result = await _service.AddStudentToPlanAsync(60, dto, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(Error.Failure, result.Error);
        }
        #endregion


        #region Tests for AddRangeOfStudentToPlanAsync

        [Fact]
        public async Task AddRangeOfStudentToPlanAsync_ShouldReturnNotFound_WhenAnyStudentMissing()
        {
            // Arrange
            var dtos = new[] { new AddStudentToPlanDto { StudentId = 1 }, new AddStudentToPlanDto { StudentId = 2 } };
            _studentServiceMock
                .SetupSequence(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(new StudentDTO()))!
                .ReturnsAsync((Result<StudentDTO>)null!);

            // Act
            var result = await _service.AddRangeOfStudentToPlanAsync(70, dtos, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(Error.NotFound, result.Error);
        }

        [Fact]
        public async Task AddRangeOfStudentToPlanAsync_ShouldReturnConflict_WhenAllAlreadyAssigned()
        {
            // Arrange
            var dtos = new List<AddStudentToPlanDto> { new() { StudentId = 3 } };
            _studentServiceMock
                .Setup(s => s.GetByIdAsync(3, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(new StudentDTO()));
            _repoMock
                .Setup(r => r.GetStudentIdsInPlanAsync(80, It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<int> { 3 });

            // Act
            var result = await _service.AddRangeOfStudentToPlanAsync(80, dtos, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(Error.Conflict, result.Error);
        }

        [Fact]
        public async Task AddRangeOfStudentToPlanAsync_ShouldReturnSuccess_WhenNewStudents()
        {
            // Arrange
            var dtos = new List<AddStudentToPlanDto>
    {
        new() { StudentId = 4 },
        new() { StudentId = 5 }
    };
            var entities = new List<PlanStudent>
    {
        new() { StudentId = 4 },
        new() { StudentId = 5 }
    };
            var resultDtos = new List<PlanStudentDto>
    {
        new() { StudentId = 4 },
        new() { StudentId = 5 }
    };

            _studentServiceMock
                .Setup(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(new StudentDTO()));
            _repoMock
                .Setup(r => r.GetStudentIdsInPlanAsync(90, It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<int>());
            _mapperMock
                .Setup(m => m.Map<IEnumerable<AddStudentToPlanDto>, List<PlanStudent>>(dtos))
                .Returns(entities);
            _repoMock
                .Setup(r => r.AddRangeAsync(entities, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _uowMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mapperMock
                .Setup(m => m.Map<List<PlanStudent>, List<PlanStudentDto>>(entities))
                .Returns(resultDtos);

            // Act
            var result = await _service.AddRangeOfStudentToPlanAsync(90, dtos, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            // unwrap the DataResult<T>
            var data = result.Value;
            Assert.Equal(2, data.TotalRecordsCount);
            Assert.Equal(resultDtos, data.Data.ToList());

            _repoMock.Verify(r => r.AddRangeAsync(entities, It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddRangeOfStudentToPlanAsync_ShouldReturnEFCoreFailure_WhenSaveFails()
        {
            // Arrange
            var dtos = new List<AddStudentToPlanDto> { new() { StudentId = 6 } };
            _studentServiceMock
                .Setup(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(new StudentDTO()));
            _repoMock
                .Setup(r => r.GetStudentIdsInPlanAsync(100, It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<int>());
            var entities = new List<PlanStudent> { new() { StudentId = 6 } };
            _mapperMock
                .Setup(m => m.Map<IEnumerable<AddStudentToPlanDto>, List<PlanStudent>>(dtos))
                .Returns(entities);
            _repoMock
                .Setup(r => r.AddRangeAsync(entities, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _uowMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.AddRangeOfStudentToPlanAsync(100, dtos, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(Error.CanNotSaveChanges, result.Error);
        }

        [Fact]
        public async Task AddRangeOfStudentToPlanAsync_ShouldReturnCancelled_WhenTokenCancelled()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            var result = await _service.AddRangeOfStudentToPlanAsync(110, new List<AddStudentToPlanDto>(), cts.Token);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(Error.OperationCancelled, result.Error);
        }

        [Fact]
        public async Task AddRangeOfStudentToPlanAsync_ShouldReturnFailure_OnException()
        {
            // Arrange
            var dtos = new List<AddStudentToPlanDto> { new() { StudentId = 7 } };
            _studentServiceMock
                .Setup(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _service.AddRangeOfStudentToPlanAsync(120, dtos, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(Error.Failure, result.Error);
        }
        #endregion
    }
}
