using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Attendences.DTOs;
using Drosy.Application.UseCases.Attendences.Services;
using Drosy.Application.UseCases.Students.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ErrorComponents;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Moq;


namespace Drosy.Tests.Application.Attendeces.Service
{
    public class AttendencesServiceTests
    {
        private readonly Mock<IAttendencesRepository> _repoMock;
        private readonly Mock<IStudentService> _studentServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ILogger<AttendencesService>> _loggerMock;
        private readonly AttendencesService _service;

        public AttendencesServiceTests()
        {
            _repoMock = new Mock<IAttendencesRepository>();
            _studentServiceMock = new Mock<IStudentService>();
            _mapperMock = new Mock<IMapper>();
            _uowMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<AttendencesService>>();
            _service = new AttendencesService(
                _repoMock.Object,
                _studentServiceMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _uowMock.Object);
        }

        // GetByIdAsync
        [Fact]
        public async Task GetByIdAsync_ShouldReturnInvalid_WhenStudentIdLessThanOne()
        {
            var result = await _service.GetByIdAsync(1, 0, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(CommonErrors.Invalid, result.Error);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNotFound_WhenRepositoryReturnsNull()
        {
            _repoMock
                .Setup(r => r.GetByIdAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Attendence)null!);

            var result = await _service.GetByIdAsync(1, 10, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(CommonErrors.NotFound, result.Error);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnSuccess_WhenRepositoryReturnsEntity()
        {
            var entity = new Attendence { SessionId = 1, StudentId = 20 };
            var dto = new AttendenceDto { SessionId = 1, StudentId = 20 };

            _repoMock
                .Setup(r => r.GetByIdAsync(1, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);
            _mapperMock
                .Setup(m => m.Map<Attendence, AttendenceDto>(entity))
                .Returns(dto);

            var result = await _service.GetByIdAsync(1, 20, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(dto, result.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCancelled_OnCancellation()
        {
            var cts = new CancellationTokenSource(); cts.Cancel();
            var result = await _service.GetByIdAsync(1, 5, cts.Token);
            Assert.True(result.IsFailure);
            Assert.Equal(CommonErrors.OperationCancelled, result.Error);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnFailure_OnException()
        {
            _repoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("error"));

            var result = await _service.GetByIdAsync(1, 5, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(AppError.Failure, result.Error);
        }

        // GetAllForSessionAsync
        [Fact]
        public async Task GetAllForSessionAsync_ShouldReturnEmptyList_WhenNoEntities()
        {
            _repoMock
                .Setup(r => r.GetAllForSessionAsync(2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Attendence>());
            _mapperMock
                .Setup(m => m.Map<List<Attendence>, List<AttendenceDto>>(It.IsAny<List<Attendence>>()))
                .Returns(new List<AttendenceDto>());

            var result = await _service.GetAllForSessionAsync(2, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value.Data);
            Assert.Equal(0, result.Value.TotalRecordsCount);
        }

        [Fact]
        public async Task GetAllForSessionAsync_ShouldReturnCancelled_OnCancellation()
        {
            var cts = new CancellationTokenSource(); cts.Cancel();
            var result = await _service.GetAllForSessionAsync(3, cts.Token);
            Assert.True(result.IsFailure);
            Assert.Equal(CommonErrors.OperationCancelled, result.Error);
        }

        [Fact]
        public async Task GetAllForSessionAsync_ShouldReturnFailure_OnException()
        {
            _repoMock
                .Setup(r => r.GetAllForSessionAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            var result = await _service.GetAllForSessionAsync(4, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(AppError.Failure, result.Error);
        }

        // GetAllForStudentAsync
        [Fact]
        public async Task GetAllForStudentAsync_ShouldReturnInvalid_WhenStudentIdLessThanOne()
        {
            var result = await _service.GetAllForStudentAsync(1, 0, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(CommonErrors.Invalid, result.Error);
        }

        [Fact]
        public async Task GetAllForStudentAsync_ShouldReturnData_WhenValid()
        {
            var entities = new List<Attendence> { new() { SessionId = 5, StudentId = 6 } };
            var dtos = new List<AttendenceDto> { new() { SessionId = 5, StudentId = 6 } };
            _repoMock
                .Setup(r => r.GetAllForStudentAsync(5, 6, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entities);
            _mapperMock
                .Setup(m => m.Map<List<Attendence>, List<AttendenceDto>>(entities))
                .Returns(dtos);

            var result = await _service.GetAllForStudentAsync(5, 6, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value.TotalRecordsCount);
            Assert.Equal(dtos, result.Value.Data);
        }

        // ... additional tests for cancellation and exception ...

        // GetAllForSessionByStatusAsync
        [Fact]
        public async Task GetAllForSessionByStatusAsync_ShouldReturnInvalid_WhenStatusUndefined()
        {
            var invalidStatus = (AttendenceStatus)99;
            var result = await _service.GetAllForSessionByStatusAsync(1, invalidStatus, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(CommonErrors.Invalid, result.Error);
        }

        [Fact]
        public async Task GetAllForSessionByStatusAsync_ShouldReturnData_WhenValid()
        {
            var entities = new List<Attendence> { new() { SessionId = 7, StudentId = 8, Status = AttendenceStatus.Absent } };
            var dtos = new List<AttendenceDto> { new() { SessionId = 7, StudentId = 8, Status = AttendenceStatus.Absent } };
            _repoMock
                .Setup(r => r.GetAllForSessionByStatusAsync(7, AttendenceStatus.Absent, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entities);
            _mapperMock
                .Setup(m => m.Map<List<Attendence>, List<AttendenceDto>>(entities))
                .Returns(dtos);

            var result = await _service.GetAllForSessionByStatusAsync(7, AttendenceStatus.Absent, CancellationToken.None);
            Assert.True(result.IsSuccess);
            Assert.Equal(dtos.Count, result.Value.TotalRecordsCount);
        }

        // ... cancellation and exception for status ...

        // AddAsync
        [Fact]
        public async Task AddAsync_ShouldReturnConflict_WhenExists()
        {
            var dto = new AddAttendencenDto { StudentId = 9 };
            _repoMock
                .Setup(r => r.ExistsAsync(1, 9, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _service.AddAsync(1, dto, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(CommonErrors.Conflict, result.Error);
        }

        [Fact]
        public async Task AddAsync_ShouldReturnSuccess_WhenNew()
        {
            var dto = new AddAttendencenDto { StudentId = 10, Status = AttendenceStatus.Present, Note = "ok" };
            var entity = new Attendence { SessionId = 1, StudentId = 10, Status = dto.Status, Note = dto.Note };
            var outDto = new AttendenceDto { SessionId = 1, StudentId = 10, Status = dto.Status, Note = dto.Note };

            _repoMock.Setup(r => r.ExistsAsync(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<AddAttendencenDto, Attendence>(dto)).Returns(entity);
            _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<Attendence, AttendenceDto>(entity)).Returns(outDto);

            var result = await _service.AddAsync(1, dto, CancellationToken.None);
            Assert.True(result.IsSuccess);
            Assert.Equal(outDto, result.Value);
        }

        // ... tests for EF failure, cancellation, exception ...

        // AddRangeAsync, UpdateAsync, DeleteAsync tests follow similar pattern
    }
}
