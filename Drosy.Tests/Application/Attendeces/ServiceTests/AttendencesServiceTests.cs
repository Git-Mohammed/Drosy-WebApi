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
using Drosy.Domain.Shared.ErrorComponents.EFCore;
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

        #region GetByIdAsync
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
        #endregion


        #region GetAllForSessionAsync
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

        #endregion


        #region GetAllForStudentAsync
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

        [Fact]
        public async Task GetAllForStudentAsync_ShouldReturnCancelled_OnCancellation()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var result = await _service.GetAllForStudentAsync(1, 2, cts.Token);
            Assert.True(result.IsFailure);
            Assert.Equal(CommonErrors.OperationCancelled, result.Error);
        }

        [Fact]
        public async Task GetAllForStudentAsync_ShouldReturnFailure_OnException()
        {
            _repoMock
                .Setup(r => r.GetAllForStudentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("boom"));

            var result = await _service.GetAllForStudentAsync(1, 2, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(AppError.Failure, result.Error);
        }

        #endregion


        #region GetAllForSessionByStatusAsync
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

        [Fact]
        public async Task GetAllForSessionByStatusAsync_ShouldReturnCancelled_OnCancellation()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var result = await _service.GetAllForSessionByStatusAsync(1, AttendenceStatus.Present, cts.Token);
            Assert.True(result.IsFailure);
            Assert.Equal(CommonErrors.OperationCancelled, result.Error);
        }

        [Fact]
        public async Task GetAllForSessionByStatusAsync_ShouldReturnFailure_OnException()
        {
            _repoMock
                .Setup(r => r.GetAllForSessionByStatusAsync(It.IsAny<int>(), It.IsAny<AttendenceStatus>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("ouch"));

            var result = await _service.GetAllForSessionByStatusAsync(1, AttendenceStatus.Present, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(AppError.Failure, result.Error);
        }


        #endregion


        #region AddAsync
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

        [Fact]
        public async Task AddAsync_ShouldReturnEFCoreFailure_WhenSaveFails()
        {
            var dto = new AddAttendencenDto { StudentId = 11, Status = AttendenceStatus.Present, Note = "ok" };
            _repoMock.Setup(r => r.ExistsAsync(2, 11, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<AddAttendencenDto, Attendence>(dto))
                       .Returns(new Attendence { SessionId = 2, StudentId = 11, Status = dto.Status, Note = dto.Note });
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Attendence>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);

            var result = await _service.AddAsync(2, dto, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(EfCoreErrors.CanNotSaveChanges, result.Error);
        }

        [Fact]
        public async Task AddAsync_ShouldReturnCancelled_OnCancellation()
        {
            var dto = new AddAttendencenDto { StudentId = 12, Status = AttendenceStatus.Present, Note = "ok" };
            var cts = new CancellationTokenSource(); cts.Cancel();

            var result = await _service.AddAsync(3, dto, cts.Token);
            Assert.True(result.IsFailure);
            Assert.Equal(CommonErrors.OperationCancelled, result.Error);
        }

        [Fact]
        public async Task AddAsync_ShouldReturnFailure_OnException()
        {
            var dto = new AddAttendencenDto { StudentId = 13, Status = AttendenceStatus.Present, Note = "ok" };
            _repoMock.Setup(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new Exception("error!"));

            var result = await _service.AddAsync(4, dto, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(AppError.Failure, result.Error);
        }


        // AddRangeAsync, UpdateAsync, DeleteAsync tests follow similar pattern
        #endregion


        #region AddRangeAsync
        [Fact]
        public async Task AddRangeAsync_ShouldReturnConflict_WhenAllExist()
        {
             var dtos = new[] {
                new AddAttendencenDto { StudentId = 21, Status = AttendenceStatus.Present, Note = "x" },
                new AddAttendencenDto { StudentId = 22, Status = AttendenceStatus.Absent, Note = "y" }
            };
            var existing = new List<Attendence> {
            new Attendence { SessionId = 5, StudentId = 21 },
            new Attendence { SessionId = 5, StudentId = 22 }
            };

            _repoMock.Setup(r => r.GetAllForStudentBySessionAsync(5, It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(existing);

            var result = await _service.AddRangeAsync(5, dtos, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(CommonErrors.Conflict, result.Error);
        }

        [Fact]
        public async Task AddRangeAsync_ShouldReturnSuccess_WhenNewItems()
        {
            var dtos = new[] {
        new AddAttendencenDto { StudentId = 31, Status = AttendenceStatus.Present, Note = "a" },
        new AddAttendencenDto { StudentId = 32, Status = AttendenceStatus.Absent, Note = "b" }
    };
            _repoMock.Setup(r => r.GetAllForStudentBySessionAsync(6, It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<Attendence>()); // none exist
            var entities = dtos.Select(d => new Attendence { SessionId = 6, StudentId = d.StudentId, Status = d.Status, Note = d.Note }).ToList();
            _mapperMock.Setup(m => m.Map<IEnumerable<AddAttendencenDto>, List<Attendence>>(dtos)).Returns(entities);
            _repoMock.Setup(r => r.AddRangeAsync(entities, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            var outDtos = entities.Select(e => new AttendenceDto { SessionId = e.SessionId, StudentId = e.StudentId, Status = e.Status, Note = e.Note }).ToList();
            _mapperMock.Setup(m => m.Map<List<Attendence>, List<AttendenceDto>>(entities)).Returns(outDtos);

            var result = await _service.AddRangeAsync(6, dtos, CancellationToken.None);
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.TotalRecordsCount);
            Assert.Equal(outDtos, result.Value.Data);
        }

        [Fact]
        public async Task AddRangeAsync_ShouldReturnEFCoreFailure_WhenSaveFails()
        {
            var dtos = new[] { new AddAttendencenDto { StudentId = 41, Status = AttendenceStatus.Present, Note = "z" } };
            _repoMock.Setup(r => r.GetAllForStudentBySessionAsync(7, It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<Attendence>());
            var entities = new List<Attendence> { new Attendence { SessionId = 7, StudentId = 41, Status = AttendenceStatus.Present, Note = "z" } };
            _mapperMock.Setup(m => m.Map<IEnumerable<AddAttendencenDto>, List<Attendence>>(dtos)).Returns(entities);
            _repoMock.Setup(r => r.AddRangeAsync(entities, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var result = await _service.AddRangeAsync(7, dtos, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(EfCoreErrors.CanNotSaveChanges, result.Error);
        }

        [Fact]
        public async Task AddRangeAsync_ShouldReturnCancelled_OnCancellation()
        {
            var cts = new CancellationTokenSource(); cts.Cancel();
            var result = await _service.AddRangeAsync(8, new List<AddAttendencenDto>(), cts.Token);
            Assert.True(result.IsFailure);
            Assert.Equal(CommonErrors.OperationCancelled, result.Error);
        }

        [Fact]
        public async Task AddRangeAsync_ShouldReturnFailure_OnException()
        {
            _repoMock.Setup(r => r.GetAllForStudentBySessionAsync(It.IsAny<int>(), It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new Exception("x"));
            var result = await _service.AddRangeAsync(9, new[] { new AddAttendencenDto { StudentId = 51, Status = AttendenceStatus.Present, Note = "n" } }, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(AppError.Failure, result.Error);
        }

        #endregion


        #region UpdateAsync
        [Fact]
        public async Task UpdateAsync_ShouldReturnNotFound_WhenEntityMissing()
        {
            _repoMock.Setup(r => r.GetByIdAsync(10, 20, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Attendence)null!);
            var result = await _service.UpdateAsync(10, 20, new UpdateAttendencenDto { Status = AttendenceStatus.Absent, Note = "x" }, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(CommonErrors.NotFound, result.Error);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenUpdated()
        {
            var entity = new Attendence { SessionId = 11, StudentId = 21, Status = AttendenceStatus.Present, Note = "old" };
            var updateDto = new UpdateAttendencenDto { Status = AttendenceStatus.Absent, Note = "new" };
            var outDto = new AttendenceDto { SessionId = 11, StudentId = 21, Status = AttendenceStatus.Absent, Note = "new" };

            _repoMock.Setup(r => r.GetByIdAsync(11, 21, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map(updateDto, entity)).Verifiable();
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<Attendence, AttendenceDto>(entity)).Returns(outDto);

            var result = await _service.UpdateAsync(11, 21, updateDto, CancellationToken.None);
            Assert.True(result.IsSuccess);
            Assert.Equal(outDto, result.Value);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnEFCoreFailure_WhenSaveFails()
        {
            var entity = new Attendence { SessionId = 12, StudentId = 22 };
            _repoMock.Setup(r => r.GetByIdAsync(12, 22, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map(It.IsAny<UpdateAttendencenDto>(), entity));
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var result = await _service.UpdateAsync(12, 22, new UpdateAttendencenDto(), CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(EfCoreErrors.CanNotSaveChanges, result.Error);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnCancelled_OnCancellation()
        {
            var cts = new CancellationTokenSource(); cts.Cancel();
            var result = await _service.UpdateAsync(13, 23, new UpdateAttendencenDto(), cts.Token);
            Assert.True(result.IsFailure);
            Assert.Equal(CommonErrors.OperationCancelled, result.Error);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFailure_OnException()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new Exception("uhoh"));
            var result = await _service.UpdateAsync(14, 24, new UpdateAttendencenDto(), CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(AppError.Failure, result.Error);
        }

        #endregion


        #region DeleteAsync
        [Fact]
        public async Task DeleteAsync_ShouldReturnNotFound_WhenEntityMissing()
        {
            _repoMock.Setup(r => r.GetByIdAsync(15, 30, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Attendence)null!);
            var result = await _service.DeleteAsync(15, 30, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(CommonErrors.NotFound, result.Error);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenDeleted()
        {
            var entity = new Attendence { SessionId = 16, StudentId = 31 };
            _repoMock.Setup(r => r.GetByIdAsync(16, 31, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
            _repoMock.Setup(r => r.DeleteAsync(entity, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _service.DeleteAsync(16, 31, CancellationToken.None);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnEFCoreFailure_WhenSaveFails()
        {
            var entity = new Attendence { SessionId = 17, StudentId = 32 };
            _repoMock.Setup(r => r.GetByIdAsync(17, 32, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
            _repoMock.Setup(r => r.DeleteAsync(entity, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var result = await _service.DeleteAsync(17, 32, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(EfCoreErrors.CanNotSaveChanges, result.Error);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnCancelled_OnCancellation()
        {
            var cts = new CancellationTokenSource(); cts.Cancel();
            var result = await _service.DeleteAsync(18, 33, cts.Token);
            Assert.True(result.IsFailure);
            Assert.Equal(CommonErrors.OperationCancelled, result.Error);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFailure_OnException()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new Exception("err"));
            var result = await _service.DeleteAsync(19, 34, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(AppError.Failure, result.Error);
        }

        #endregion

    }
}
