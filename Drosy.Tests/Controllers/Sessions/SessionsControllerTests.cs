using Drosy.Api.Commons.Responses;
using Drosy.Api.Controllers;
using Drosy.Application.UseCases.Sessions.DTOs;
using Drosy.Application.UseCases.Sessions.Interfaces;
using Drosy.Application.UseCases.Sessions.Services;
using Drosy.Domain.Enums;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;


namespace Drosy.Tests.Controllers.Sessions
{
    public class SessionsControllerTests
    {
        private readonly Mock<ISessionService> _sessionServiceMock;
        private readonly SessionsController _controller;

        public SessionsControllerTests()
        {
            _sessionServiceMock = new Mock<ISessionService>();
            _controller = new SessionsController(_sessionServiceMock.Object);
        }

        #region GetByIdAsync

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
                _sessionServiceMock
                    .Setup(s => s.GetByIdAsync(sessionId, CancellationToken.None))
                    .ReturnsAsync(Result.Success(sessionDto));
            }
            else
            {
                _sessionServiceMock
                    .Setup(s => s.GetByIdAsync(sessionId, CancellationToken.None))
                    .ReturnsAsync(Result.Failure<SessionDTO>(CommonErrors.Invalid));
            }

            // Act
            var result = await _controller.GetByIdAsync(sessionId, CancellationToken.None);

            // Assert
            if (isValid)
            {
                var okResult = Assert.IsType<OkObjectResult>(result);  // Expect OkObjectResult for valid ID
                var response = Assert.IsType<ApiResponse<SessionDTO>>(okResult.Value);
                Assert.Equal("Session retrieved successfully.", response.Message);
            }
            else
            {
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);  // Expect BadRequestObjectResult for invalid ID
                var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
                Assert.Equal("Invalid session ID.", response.Message);
            }
        }

        #endregion

        #region GetListAsync
        [Theory]
        [InlineData(null, null, null, null, null, null, null, true)]  // Test with no filters
        [InlineData("2025-07-01", null, null, null, null, null, null, true)]  // Test with date filter
        [InlineData(null, "2025-07-01", "2025-07-31", null, null, null, null, true)]  // Test with range filter
        [InlineData(null, null, null, "Completed", null, null, null, true)]  // Test with status filter
        [InlineData(null, null, null, null, 2025, 20, null, true)]  // Test with week filter
        [InlineData(null, null, null, null, 2025, null, 7, true)]  // Test with month filter
        public async Task GetListAsync_ShouldReturnSuccess_WhenValidFiltersAreApplied(
            string date, string start, string end, string status, int? year, int? week, int? month, bool isValid)
        {
            // Convert string dates to nullable DateTime
            DateTime? parsedDate = string.IsNullOrEmpty(date) ? (DateTime?)null : DateTime.Parse(date);
            DateTime? parsedStart = string.IsNullOrEmpty(start) ? (DateTime?)null : DateTime.Parse(start);
            DateTime? parsedEnd = string.IsNullOrEmpty(end) ? (DateTime?)null : DateTime.Parse(end);
            SessionStatus? parsedStatus = string.IsNullOrEmpty(status) ? (SessionStatus?)null : Enum.Parse<SessionStatus>(status);

            // Create a mock session DTO to return
            var sessionDto = new SessionDTO { Id = 1, Title = "Filtered Session" };
            var data = new DataResult<SessionDTO> { Data = new[] { sessionDto }, TotalRecordsCount = 1 };

            // Mock setup for GetSessionsByDate, GetSessionsInRange, GetSessionsByStatus, etc.
            if (parsedDate.HasValue)
            {
                _sessionServiceMock.Setup(s => s.GetSessionsByDate(It.IsAny<DateTime>(), CancellationToken.None))
                                   .ReturnsAsync(Result.Success(data));
            }

            if (parsedStart.HasValue && parsedEnd.HasValue)
            {
                _sessionServiceMock.Setup(s => s.GetSessionsInRange(It.IsAny<DateTime>(), It.IsAny<DateTime>(), CancellationToken.None))
                                   .ReturnsAsync(Result.Success(data));
            }

            if (parsedStatus.HasValue)
            {
                _sessionServiceMock.Setup(s => s.GetSessionsByStatus(It.IsAny<SessionStatus>(), CancellationToken.None))
                                   .ReturnsAsync(Result.Success(data));
            }

            // Setup the mock for GetSessionsByWeek if year and week are provided
            if (year.HasValue && week.HasValue)
            {
                _sessionServiceMock.Setup(s => s.GetSessionsByWeek(It.IsAny<int>(), It.IsAny<int>(), CancellationToken.None))
                                   .ReturnsAsync(Result.Success(data));
            }

            // Setup the mock for GetSessionsByMonth if year and month are provided
            if (year.HasValue && month.HasValue)
            {
                _sessionServiceMock.Setup(s => s.GetSessionsByMonth(It.IsAny<int>(), It.IsAny<int>(), CancellationToken.None))
                                   .ReturnsAsync(Result.Success(data));
            }

            if (!parsedDate.HasValue && !parsedStart.HasValue && !parsedEnd.HasValue && !parsedStatus.HasValue && !year.HasValue && !week.HasValue && !month.HasValue)
            {
                _sessionServiceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                                   .ReturnsAsync(Result.Success(data));
            }

            // Act: Call the method
            var result = await _controller.GetListAsync(
                date: parsedDate,
                start: parsedStart,
                end: parsedEnd,
                status: parsedStatus,
                year: year,
                week: week,
                month: month,
                ct: CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);  // Expect OkObjectResult for valid data
            var response = Assert.IsType<ApiResponse<DataResult<SessionDTO>>>(okResult.Value);
            Assert.True(response.IsSuccess);
        }


        #endregion

        #region CreateAsync

        [Theory]
        [InlineData(null, false)]  // Null DTO
        [InlineData("Valid Session", true)]  // Valid DTO
        public async Task CreateAsync_ShouldReturnResponse_WhenValidAndInvalidData(string title, bool isValid)
        {
            var dto = new CreateSessionDTO { Title = title, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1), ExcepectedDate = DateTime.Now };

            if (isValid)
            {
                var createdSession = new SessionDTO { Id = 1, Title = title };
                _sessionServiceMock.Setup(s => s.CreateAsync(dto, CancellationToken.None))
                                   .ReturnsAsync(Result.Success(createdSession));

                // Act
                var result = await _controller.CreateAsync(dto, CancellationToken.None);

                var createdResult = Assert.IsType<CreatedAtRouteResult>(result);  // Expect CreatedAtRouteResult for successful creation
                var response = Assert.IsType<ApiResponse<SessionDTO>>(createdResult.Value);
                Assert.Equal("Session added successfully.", response.Message);
            }
            else
            {
                _sessionServiceMock.Setup(s => s.CreateAsync(dto, CancellationToken.None))
                                   .ReturnsAsync(Result.Failure<SessionDTO>(CommonErrors.Invalid));

                // Act
                var result = await _controller.CreateAsync(dto, CancellationToken.None);

                var unprocessableEntityResult = Assert.IsType<UnprocessableEntityObjectResult>(result);  // Corrected to UnprocessableEntityObjectResult
                var response = Assert.IsType<ApiResponse<object>>(unprocessableEntityResult.Value);
                Assert.Equal("Session creation failed.", response.Message);
            }
        }
        #endregion

        #region RescheduleAsync

        [Theory]
        [InlineData(0, null, false)]  // Invalid ID or Null DTO
        [InlineData(1, "2025-07-01", true)]  // Valid ID and DTO
        public async Task RescheduleAsync_ShouldReturnResponse_WhenValidAndInvalidData(int sessionId, string newDate, bool isValid)
        {
            if (string.IsNullOrEmpty(newDate))
            {
                // Handle the null/empty date gracefully, e.g., provide a default value or skip parsing
                newDate = "2025-07-01"; // Provide a default valid date
            }

            var dto = new RescheduleSessionDTO
            {
                NewStartTime = DateTime.Now,
                NewEndTime = DateTime.Now.AddHours(1),
                NewDate = string.IsNullOrEmpty(newDate) ? DateTime.MinValue : DateTime.Parse(newDate)  // Avoid null DateTime
            };

            if (isValid)
            {
                var session = new SessionDTO { Id = sessionId, Title = "Rescheduled Session" };
                _sessionServiceMock.Setup(s => s.RescheduleAsync(sessionId, dto, CancellationToken.None))
                                   .ReturnsAsync(Result.Success(session));

                // Act
                var result = await _controller.RescheduleAsync(sessionId, dto, CancellationToken.None);

                var okResult = Assert.IsType<OkObjectResult>(result);
                var response = Assert.IsType<ApiResponse<SessionDTO>>(okResult.Value);
                Assert.Equal("Session rescheduled successfully.", response.Message);
            }
            else
            {
                _sessionServiceMock.Setup(s => s.RescheduleAsync(sessionId, dto, CancellationToken.None))
                                   .ReturnsAsync(Result.Failure<SessionDTO>(CommonErrors.Invalid));

                // Act
                var result = await _controller.RescheduleAsync(sessionId, dto, CancellationToken.None);

                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
                Assert.Equal("Invalid session ID.", response.Message);
            }
        }

        #endregion
    }
}
