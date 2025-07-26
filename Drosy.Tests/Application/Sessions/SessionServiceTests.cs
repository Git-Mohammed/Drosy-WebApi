using Drosy.Application.UseCases.Sessions.DTOs;
using Drosy.Application.UseCases.Sessions.Interfaces;
using Drosy.Domain.Enums;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Drosy.Tests.Application.Sessions
{
    public class SessionServiceMockTests
    {
        private readonly Mock<ISessionService> _sessionService;

        public SessionServiceMockTests()
        {
            _sessionService = new Mock<ISessionService>();
        }

        [Theory]
        [InlineData(0, false)]  // Invalid ID
        [InlineData(-5, false)] // Negative ID
        [InlineData(1, true)]   // Valid ID
        public async Task GetByIdAsync_ValidationTheory(int sessionId, bool isValid)
        {
            // Arrange
            if (isValid)
            {
                var sessionDto = new SessionDTO
                {
                    Id = sessionId,
                    Title = "Test Session",
                    StartTime = DateTime.Today.AddHours(10),
                    EndTime = DateTime.Today.AddHours(11),
                    ExcepectedDate = DateTime.Today
                };

                _sessionService
                    .Setup(s => s.GetByIdAsync(sessionId, CancellationToken.None))
                    .ReturnsAsync(Result.Success(sessionDto));
            }
            else
            {
                _sessionService
                    .Setup(s => s.GetByIdAsync(sessionId, CancellationToken.None))
                    .ReturnsAsync(Result.Failure<SessionDTO>(CommonErrors.Invalid));
            }

            // Act
            var result = await _sessionService.Object.GetByIdAsync(sessionId, CancellationToken.None);

            // Assert
            if (isValid)
            {
                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Value);
                Assert.Equal(sessionId, result.Value.Id);
            }
            else
            {
                Assert.False(result.IsSuccess);
                Assert.NotNull(result.Error);
            }
        }


        [Theory]
        [InlineData("", "2025-07-24T10:00:00", "2025-07-24T11:00:00", "2025-07-24", false)]  // Title empty
        [InlineData("Valid Session", "2025-07-24T11:00:00", "2025-07-24T10:00:00", "2025-07-24", false)] // Start after end
        [InlineData("Valid Session", "2025-07-25T10:00:00", "2025-07-25T11:00:00", "2025-07-24", false)] // Outside expected date
        [InlineData("Valid Session", "2025-07-24T10:00:00", "2025-07-24T11:00:00", "2025-07-24", true)]  // Valid input
        public async Task AddAsync_ValidationTheory(string title, string start, string end, string expected, bool isValid)
        {
            // Arrange
            var addDto = new CreateSessionDTO
            {
                Title = title,
                StartTime = DateTime.Parse(start),
                EndTime = DateTime.Parse(end),
                ExcepectedDate = DateTime.Parse(expected)
            };

            if (isValid)
            {
                var resultDto = new SessionDTO { Id = 1, Title = title };
                _sessionService
                    .Setup(s => s.CreateAsync(It.Is<CreateSessionDTO>(d => d.Title == title), CancellationToken.None))
                    .ReturnsAsync(Result.Success(resultDto));
            }
            else
            {
                _sessionService
                    .Setup(s => s.CreateAsync(It.Is<CreateSessionDTO>(d => d.Title == title), CancellationToken.None))
                    .ReturnsAsync(Result.Failure<SessionDTO>(CommonErrors.Invalid));
            }

            // Act
            var result = await _sessionService.Object.CreateAsync(addDto, CancellationToken.None);

            // Assert
            if (isValid)
            {
                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Value);
            }
            else
            {
                Assert.False(result.IsSuccess);
                Assert.NotNull(result.Error);
            }
        }

        [Theory]
        [InlineData(0, "2025-08-01T09:00:00", "2025-08-01T10:00:00", "2025-08-01", false)]  // Invalid ID
        [InlineData(1, "2025-08-01T10:30:00", "2025-08-01T09:30:00", "2025-08-01", false)] // StartTime after EndTime
        [InlineData(1, "2025-08-02T09:00:00", "2025-08-02T10:00:00", "2025-08-01", false)] // Date mismatch
        [InlineData(1, "2025-08-01T09:00:00", "2025-08-01T10:00:00", "2025-08-01", true)]  // Valid input
        public async Task RescheduleAsync_ValidationTheory(int sessionId, string start, string end, string expected, bool isValid)
        {
            // Arrange
            var dto = new RescheduleSessionDTO
            {
                NewStartTime = DateTime.Parse(start),
                NewEndTime = DateTime.Parse(end),
                NewDate = DateTime.Parse(expected)
            };

            if (isValid)
            {
                var sessionResult = new SessionDTO { Id = sessionId, Title = "Updated Session" };
                _sessionService
                    .Setup(s => s.RescheduleAsync(
                        It.Is<int>(id => id == sessionId),
                        It.Is<RescheduleSessionDTO>(d =>
                            d.NewStartTime == dto.NewStartTime &&
                            d.NewEndTime == dto.NewEndTime &&
                            d.NewDate == dto.NewDate),
                        CancellationToken.None))
                    .ReturnsAsync(Result.Success(sessionResult));
            }
            else
            {
                _sessionService
                    .Setup(s => s.RescheduleAsync(
                        It.Is<int>(id => id == sessionId),
                        It.Is<RescheduleSessionDTO>(d =>
                            d.NewStartTime == dto.NewStartTime &&
                            d.NewEndTime == dto.NewEndTime &&
                            d.NewDate == dto.NewDate),
                        CancellationToken.None))
                    .ReturnsAsync(Result.Failure<SessionDTO>(CommonErrors.Invalid));
            }

            // Act
            var result = await _sessionService.Object.RescheduleAsync(sessionId, dto, CancellationToken.None);

            // Assert
            if (isValid)
            {
                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Value);
                Assert.Equal(sessionId, result.Value.Id);
            }
            else
            {
                Assert.False(result.IsSuccess);
                Assert.NotNull(result.Error);
                Assert.Equal(CommonErrors.Invalid.Code, result.Error.Code);
            }
        }


        #region GetSessionsByDate

        [Fact]
        public async Task GetSessionsByDate_ShouldReturnSuccess_WhenDataIsReturned()
        {
            // Arrange
            var date = DateTime.Today;
            var dto = new SessionDTO { Id = 1, Title = "A" };
            var data = new DataResult<SessionDTO> { Data = new[] { dto }, TotalRecordsCount = 1 };
            _sessionService
                .Setup(s => s.GetSessionsByDate(date, CancellationToken.None))
                .ReturnsAsync(Result.Success(data));

            // Act
            var result = await _sessionService.Object.GetSessionsByDate(date, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value.TotalRecordsCount);
            Assert.Single(result.Value.Data);
        }

        [Fact]
        public async Task GetSessionsByDate_ShouldReturnFailure_WhenServiceFails()
        {
            // Arrange
            var date = DateTime.Today;
            _sessionService
                .Setup(s => s.GetSessionsByDate(date, CancellationToken.None))
                .ReturnsAsync(Result.Failure<DataResult<SessionDTO>>(CommonErrors.Invalid));

            // Act
            var result = await _sessionService.Object.GetSessionsByDate(date, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
        }
        #endregion

        #region GetSessionsInRange

        [Theory]
        [InlineData("2025-07-10", "2025-07-09", false)] // end before start
        [InlineData("2025-07-10", "2025-07-11", true)]  // valid
        public async Task GetSessionsInRange_ValidationTheory(string start, string end, bool isValid)
        {
            var s = DateTime.Parse(start);
            var e = DateTime.Parse(end);

            if (isValid)
            {
                var dto = new SessionDTO { Id = 2, Title = "B" };
                var data = new DataResult<SessionDTO> { Data = new[] { dto }, TotalRecordsCount = 1 };
                _sessionService
                    .Setup(x => x.GetSessionsInRange(s, e, CancellationToken.None))
                    .ReturnsAsync(Result.Success(data));
            }
            else
            {
                _sessionService
                    .Setup(x => x.GetSessionsInRange(s, e, CancellationToken.None))
                    .ReturnsAsync(Result.Failure<DataResult<SessionDTO>>(CommonErrors.Invalid));
            }

            var result = await _sessionService.Object.GetSessionsInRange(s, e, CancellationToken.None);

            Assert.Equal(isValid, result.IsSuccess);
        }

        #endregion
        #region GetSessionsByWeek

        [Theory]
        [InlineData(2025, 0, false)]
        [InlineData(2025, 54, false)]
        [InlineData(2025, 30, true)]
        public async Task GetSessionsByWeek_ValidationTheory(int year, int week, bool isValid)
        {
            if (isValid)
            {
                var dto = new SessionDTO { Id = 3, Title = "C" };
                var data = new DataResult<SessionDTO> { Data = new[] { dto }, TotalRecordsCount = 1 };
                _sessionService
                    .Setup(x => x.GetSessionsByWeek(year, week, CancellationToken.None))
                    .ReturnsAsync(Result.Success(data));
            }
            else
            {
                _sessionService
                    .Setup(x => x.GetSessionsByWeek(year, week, CancellationToken.None))
                    .ReturnsAsync(Result.Failure<DataResult<SessionDTO>>(CommonErrors.Invalid));
            }

            var result = await _sessionService.Object.GetSessionsByWeek(year, week, CancellationToken.None);
            Assert.Equal(isValid, result.IsSuccess);
        }

        #endregion

        #region GetSessionsByMonth

        [Theory]
        [InlineData(2025, 0, false)]
        [InlineData(2025, 13, false)]
        [InlineData(2025, 7, true)]
        public async Task GetSessionsByMonth_ValidationTheory(int year, int month, bool isValid)
        {
            if (isValid)
            {
                var dto = new SessionDTO { Id = 4, Title = "D" };
                var data = new DataResult<SessionDTO> { Data = new[] { dto }, TotalRecordsCount = 1 };
                _sessionService
                    .Setup(x => x.GetSessionsByMonth(year, month, CancellationToken.None))
                    .ReturnsAsync(Result.Success(data));
            }
            else
            {
                _sessionService
                    .Setup(x => x.GetSessionsByMonth(year, month, CancellationToken.None))
                    .ReturnsAsync(Result.Failure<DataResult<SessionDTO>>(CommonErrors.Invalid));
            }

            var result = await _sessionService.Object.GetSessionsByMonth(year, month, CancellationToken.None);
            Assert.Equal(isValid, result.IsSuccess);
        }

        #endregion

        #region GetSessionsByStatus

        [Theory]
        [InlineData((SessionStatus)99, false)]
        [InlineData(SessionStatus.Completed, true)]
        public async Task GetSessionsByStatus_ValidationTheory(SessionStatus status, bool isValid)
        {
            if (isValid)
            {
                var dto = new SessionDTO { Id = 5, Title = "E" };
                var data = new DataResult<SessionDTO> { Data = new[] { dto }, TotalRecordsCount = 1 };
                _sessionService
                    .Setup(x => x.GetSessionsByStatus(status, CancellationToken.None))
                    .ReturnsAsync(Result.Success(data));
            }
            else
            {
                _sessionService
                    .Setup(x => x.GetSessionsByStatus(status, CancellationToken.None))
                    .ReturnsAsync(Result.Failure<DataResult<SessionDTO>>(CommonErrors.Invalid));
            }

            var result = await _sessionService.Object.GetSessionsByStatus(status, CancellationToken.None);
            Assert.Equal(isValid, result.IsSuccess);
        }

        #endregion



    }
}
