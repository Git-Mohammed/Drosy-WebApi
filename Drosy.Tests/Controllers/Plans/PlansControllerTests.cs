using Drosy.Api.Commons.Responses;
using Drosy.Api.Controllers;
using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Plans.Interfaces;
using Drosy.Application.UseCases.Sessions.DTOs;
using Drosy.Application.UseCases.Sessions.Interfaces;
using Drosy.Domain.Enums;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ErrorComponents.Plans;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;
using Drosy.Application.UseCases.PlanStudents.DTOs;

namespace Drosy.Tests.Controllers.Plans
{
    public class PlansControllerTests
    {
        private readonly Mock<IPlanService> _planServiceMock;
        private readonly Mock<ISessionService> _sessionServiceMock;
        private readonly PlansController _controller;
        private readonly DataResult<CalenderSessionDto> _sampleCalendarData;
        private readonly DataResult<PlanDto> _samplePlanData;
        private readonly DataResult<SessionDTO> _sampleSessionData;

        public PlansControllerTests()
        {
            // Initialize mocks for the plan and session services
            _planServiceMock = new Mock<IPlanService>(MockBehavior.Strict); // Strict to catch unconfigured calls
            _sessionServiceMock = new Mock<ISessionService>(MockBehavior.Strict); // Strict to catch unconfigured calls

            // Initialize the PlansController with mocked services and a null logger
            _controller = new PlansController(_planServiceMock.Object, _sessionServiceMock.Object, NullLogger<PlansController>.Instance);
            // Initialize sample data for testing
            var sampleCalendar = new CalenderSessionDto
            {
                PlanId = 123,
                PlanType = PlanTypes.Individual.ToString(),
                PlanStatus = PlanStatus.Active.ToString(),
                ExcepectedDate = new DateTime(2025, 8, 15),
                SessionPeriod = 60,
                Period = 12,
                Days = new List<PlanDayDto>
                {
                    new PlanDayDto { Day = "Monday", StartSession = TimeSpan.FromHours(9), EndSession = TimeSpan.FromHours(10) },
                    new PlanDayDto { Day = "Wednesday", StartSession = TimeSpan.FromHours(9), EndSession = TimeSpan.FromHours(10) }
                },
                Students = new List<CalenderPlanStudentDto>
                {
                    new CalenderPlanStudentDto
                    {
                        StudentId = 42,
                        Notes = "On time",
                        Fee = 200m,
                        CreatedAt = DateTime.UtcNow,
                        FullName = "Jane Doe",
                        Address = "123 Main St",
                        PhoneNumber = "555-0101"
                    }
                }
            };

            var samplePlan = new PlanDto
            {
                Id = 123,
                Type = PlanTypes.Individual.ToString(),
                Status = PlanStatus.Active.ToString(),
                TotalFees = 1000m,
                StartDate = new DateTime(2025, 8, 1),
                EndDate = new DateTime(2025, 8, 31),
                Days = new List<PlanDayDto>
                {
                    new PlanDayDto { Day = "Monday", StartSession = TimeSpan.FromHours(9), EndSession = TimeSpan.FromHours(10) }
                }
            };

            var sampleSession = new SessionDTO
            {
                Id = 1,
                Title = "Sample Session",
                StartTime = new DateTime(2025, 8, 15, 9, 0, 0),
                EndTime = new DateTime(2025, 8, 15, 10, 0, 0),
                ExcepectedDate = new DateTime(2025, 8, 15)
            };

            _sampleCalendarData = new DataResult<CalenderSessionDto>
            {
                Data = new[] { sampleCalendar },
                TotalRecordsCount = 1
            };

            _samplePlanData = new DataResult<PlanDto>
            {
                Data = new[] { samplePlan },
                TotalRecordsCount = 1
            };

            _sampleSessionData = new DataResult<SessionDTO>
            {
                Data = new[] { sampleSession },
                TotalRecordsCount = 1
            };
        }

        #region GetByIdAsync

        //[Theory]
        //[InlineData(0, false)]  // Invalid ID
        //[InlineData(-1, false)] // Negative ID
        //[InlineData(1, true)]   // Valid ID
        //public async Task GetByIdAsync_ValidationTheory(int planId, bool isValid)
        //{
        //    // Arrange
        //    if (isValid)
        //    {
        //        var planDto = new PlanDto
        //        {
        //            Id = planId,
        //            Type = PlanTypes.Individual.ToString(),
        //            Status = PlanStatus.Active.ToString(),
        //            StartDate = DateTime.Today,
        //            EndDate = DateTime.Today.AddDays(7),
        //            Days = new List<PlanDayDto>()
        //        };
        //        _planServiceMock.Setup(s => s.GetPlanByIdAsync(planId, It.IsAny<CancellationToken>()))
        //            .ReturnsAsync(Result.Success(planDto));
        //    }
        //    else
        //    {
        //        _planServiceMock.Setup(s => s.GetPlanByIdAsync(planId, It.IsAny<CancellationToken>()))
        //            .ReturnsAsync(Result.Failure<PlanDto>(PlanErrors.PlanNotFound));
        //    }

        //    // Act
        //    var result = await _controller.GetByIdAsync(planId, CancellationToken.None);

        //    // Assert
        //    if (isValid)
        //    {
        //        var okResult = Assert.IsType<OkObjectResult>(result);
        //        var response = Assert.IsType<ApiResponse<PlanDto>>(okResult.Value);
        //        Assert.Equal("Request successful", response.Message); // Updated to match actual response
        //        Assert.Equal(planId, response.Data.Id);
        //    }
        //    else
        //    {
        //        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        //        var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
        //        Assert.Equal("Plan not found.", response.Message);
        //    }
        //}
        #endregion

        #region GetListAsync

        [Fact]
        public async Task GetListAsync_WeekWithoutYear_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetListAsync(null, null, null, null, null, null, 10, null, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Contains("week", response.Errors![0].Property);
            Assert.Equal("`week` requires `year`.", response.Errors[0].Message);
        }

        [Fact]
        public async Task GetListAsync_MonthWithoutYear_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetListAsync(null, null, null, null, null, null, null, 12, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Contains("month", response.Errors![0].Property);
            Assert.Equal("`month` requires `year`.", response.Errors[0].Message);
        }

        [Fact]
        public async Task GetListAsync_BothWeekAndMonth_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetListAsync(null, null, null, null, null, 2025, 10, 12, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Contains("filters", response.Errors![0].Property);
            Assert.Equal("Cannot filter by both `week` and `month`.", response.Errors[0].Message);
        }

        #endregion

        #region GetPlanSessionsAsync

        [Fact]
        public async Task GetPlanSessionsAsync_WeekWithoutYear_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetPlanSessionsAsync(1, null, null, null, null, null, 10, null, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Contains("week", response.Errors![0].Property);
            Assert.Equal("`week` requires `year`.", response.Errors[0].Message);
        }

        [Fact]
        public async Task GetPlanSessionsAsync_MonthWithoutYear_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetPlanSessionsAsync(1, null, null, null, null, null, null, 12, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Contains("month", response.Errors![0].Property);
            Assert.Equal("`month` requires `year`.", response.Errors[0].Message);
        }

        [Fact]
        public async Task GetPlanSessionsAsync_BothWeekAndMonth_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetPlanSessionsAsync(1, null, null, null, null, 2025, 10, 12, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Contains("filters", response.Errors![0].Property);
            Assert.Equal("Cannot filter by both `week` and `month`.", response.Errors[0].Message);
        }

        [Theory]
        [InlineData(1, "2025-08-01", null, null, null, null, null, null, true)]  // Date filter
        [InlineData(1, null, "2025-08-01", "2025-08-07", null, null, null, null, true)]  // Range filter
        [InlineData(1, null, null, null, "Scheduled", null, null, null, true)]  // Status filter (using a placeholder valid SessionStatus)
        [InlineData(1, null, null, null, null, 2025, 32, null, true)]  // Week filter
        [InlineData(1, null, null, null, null, 2025, null, 8, true)]  // Month filter
        [InlineData(1, null, null, null, null, null, null, null, true)]  // No filters
        public async Task GetPlanSessionsAsync_ValidFilters_ReturnsOk(
        int planId, string date, string start, string end, string status, int? year, int? week, int? month, bool isValid)
        {
            // Arrange
            DateTime? parsedDate = string.IsNullOrEmpty(date) ? null : DateTime.Parse(date);
            DateTime? parsedStart = string.IsNullOrEmpty(start) ? null : DateTime.Parse(start);
            DateTime? parsedEnd = string.IsNullOrEmpty(end) ? null : DateTime.Parse(end);
            SessionStatus? parsedStatus = string.IsNullOrEmpty(status) ? null : Enum.Parse<SessionStatus>(status); // Ensure status matches SessionStatus enum

            if (isValid)
            {
                if (parsedDate.HasValue)
                {
                    _sessionServiceMock.Setup(s => s.GetSessionsByDate(planId, parsedDate.Value, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result.Success(_sampleSessionData));
                }
                else if (parsedStart.HasValue && parsedEnd.HasValue)
                {
                    _sessionServiceMock.Setup(s => s.GetSessionsInRange(planId, parsedStart.Value, parsedEnd.Value, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result.Success(_sampleSessionData));
                }
                else if (parsedStatus.HasValue)
                {
                    _sessionServiceMock.Setup(s => s.GetSessionsByStatus(planId, parsedStatus.Value, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result.Success(_sampleSessionData));
                }
                else if (year.HasValue && week.HasValue)
                {
                    _sessionServiceMock.Setup(s => s.GetSessionsByWeek(planId, year.Value, week.Value, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result.Success(_sampleSessionData));
                }
                else if (year.HasValue && month.HasValue)
                {
                    _sessionServiceMock.Setup(s => s.GetSessionsByMonth(planId, year.Value, month.Value, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result.Success(_sampleSessionData));
                }
                else
                {
                    _sessionServiceMock.Setup(s => s.GetSessionsByPlan(planId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result.Success(_sampleSessionData));
                }
            }

            // Act
            var result = await _controller.GetPlanSessionsAsync(planId, parsedDate, parsedStart, parsedEnd, parsedStatus, year, week, month, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<DataResult<SessionDTO>>>(okResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal(1, response.Data.TotalRecordsCount);
        }
        #endregion

        #region GetPlanCalendarSessionsAsync

        [Fact]
        public async Task GetPlanCalendarSessionsAsync_WeekWithoutYear_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetPlanCalendarSessionsAsync(1, null, null, null, null, null, 10, null, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Contains("week", response.Errors![0].Property);
            Assert.Equal("`week` requires `year`.", response.Errors[0].Message);
        }

        [Fact]
        public async Task GetPlanCalendarSessionsAsync_MonthWithoutYear_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetPlanCalendarSessionsAsync(1, null, null, null, null, null, null, 12, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Contains("month", response.Errors![0].Property);
            Assert.Equal("`month` requires `year`.", response.Errors[0].Message);
        }

        [Fact]
        public async Task GetPlanCalendarSessionsAsync_BothWeekAndMonth_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetPlanCalendarSessionsAsync(1, null, null, null, null, 2025, 10, 12, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Contains("filters", response.Errors![0].Property);
            Assert.Equal("Cannot filter by both `week` and `month`.", response.Errors[0].Message);
        }

        [Theory]
        [InlineData(1, "2025-08-01", null, null, null, null, null, null, true)]  // Date filter
        [InlineData(1, null, "2025-08-01", "2025-08-07", null, null, null, null, true)]  // Range filter
        [InlineData(1, null, null, null, "Active", null, null, null, true)]  // Status filter
        [InlineData(1, null, null, null, null, 2025, 32, null, true)]  // Week filter
        [InlineData(1, null, null, null, null, 2025, null, 8, true)]  // Month filter
        [InlineData(1, null, null, null, null, null, null, null, true)]  // No filters
        public async Task GetPlanCalendarSessionsAsync_ValidFilters_ReturnsOk(
            int planId, string date, string start, string end, string status, int? year, int? week, int? month, bool isValid)
        {
            // Arrange
            DateTime? parsedDate = string.IsNullOrEmpty(date) ? null : DateTime.Parse(date);
            DateTime? parsedStart = string.IsNullOrEmpty(start) ? null : DateTime.Parse(start);
            DateTime? parsedEnd = string.IsNullOrEmpty(end) ? null : DateTime.Parse(end);
            PlanStatus? parsedStatus = string.IsNullOrEmpty(status) ? null : Enum.Parse<PlanStatus>(status);

            if (isValid)
            {
                if (parsedDate.HasValue)
                {
                    _planServiceMock.Setup(s => s.GetPlanSessionsCalenderByDateAsync(planId, parsedDate.Value, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result.Success(_sampleCalendarData));
                }
                else if (parsedStart.HasValue && parsedEnd.HasValue)
                {
                    _planServiceMock.Setup(s => s.GetPlanSessionsCalenderByRangeAsync(planId, parsedStart.Value, parsedEnd.Value, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result.Success(_sampleCalendarData));
                }
                else if (parsedStatus.HasValue)
                {
                    _planServiceMock.Setup(s => s.GetPlanSessionsCalenderByStatusAsync(planId, parsedStatus.Value, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result.Success(_sampleCalendarData));
                }
                else if (year.HasValue && week.HasValue)
                {
                    _planServiceMock.Setup(s => s.GetPlanSessionsCalenderByWeekAsync(planId, year.Value, week.Value, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result.Success(_sampleCalendarData));
                }
                else if (year.HasValue && month.HasValue)
                {
                    _planServiceMock.Setup(s => s.GetPlanSessionsCalenderByMonthAsync(planId, year.Value, month.Value, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result.Success(_sampleCalendarData));
                }
                else
                {
                    _planServiceMock.Setup(s => s.GetPlanSessionsCalenderAsync(planId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result.Success(_sampleCalendarData));
                }
            }

            // Act
            var result = await _controller.GetPlanCalendarSessionsAsync(planId, parsedDate, parsedStart, parsedEnd, parsedStatus, year, week, month, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<DataResult<CalenderSessionDto>>>(okResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal(1, response.Data.TotalRecordsCount);
        }

        [Fact]
        public async Task GetPlanCalendarSessionsAsync_PlanNotFound_ReturnsBadRequest()
        {
            // Arrange
            _planServiceMock.Setup(s => s.GetPlanSessionsCalenderAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<DataResult<CalenderSessionDto>>(PlanErrors.PlanNotFound));

            // Act
            var result = await _controller.GetPlanCalendarSessionsAsync(1, null, null, null, null, null, null, null, CancellationToken.None);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode); // Verify BadRequest status code
            var response = Assert.IsType<ApiResponse<object>>(objectResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Equal("حدث خطأ غير متوقع.", response.Message);
        }

        #endregion

        #region CreateAsync

        //[Theory]
        //[InlineData(null, false)]  // Null DTO
        //[InlineData("Individual", true)]  // Valid DTO
        //public async Task CreateAsync_ValidatesInput(string type, bool isValid)
        //{
        //    // Arrange
        //    var createDto = type == null ? null : new CreatePlanDto
        //    {
        //        Type = type,
        //        Status = PlanStatus.Active.ToString(),
        //        TotalFees = 100,
        //        StartDate = DateTime.Today,
        //        Period = 30,
        //        Days = new List<PlanDayDto> { new PlanDayDto { Day = "Monday", StartSession = TimeSpan.FromHours(9), EndSession = TimeSpan.FromHours(10) } }
        //    };

        //    if (isValid)
        //    {
        //        var planDto = new PlanDto
        //        {
        //            Id = 1,
        //            Type = type,
        //            Status = PlanStatus.Active.ToString(),
        //            TotalFees = 100,
        //            StartDate = DateTime.Today,
        //            EndDate = DateTime.Today.AddDays(30),
        //            Days = createDto.Days
        //        };
        //        _planServiceMock.Setup(s => s.CreatePlanAsync(It.Is<CreatePlanDto>(d => d.Type == type), It.IsAny<CancellationToken>()))
        //            .ReturnsAsync(Result.Success(planDto));
        //    }
        //    else
        //    {
        //        _planServiceMock.Setup(s => s.CreatePlanAsync(It.IsAny<CreatePlanDto>(), It.IsAny<CancellationToken>()))
        //            .ReturnsAsync(Result.Failure<PlanDto>(PlanErrors.PlanSaveFailure));
        //    }

        //    // Act
        //    var result = await _controller.CreateAsync(createDto, CancellationToken.None);

        //    // Assert
        //    if (isValid)
        //    {
        //        var createdResult = Assert.IsType<CreatedAtRouteResult>(result);
        //        var response = Assert.IsType<ApiResponse<PlanDto>>(createdResult.Value);
        //        Assert.Equal("Plan added successfully.", response.Message);
        //        Assert.Equal(1, response.Data.Id);
        //    }
        //    else
        //    {
        //        var unprocessableEntityResult = Assert.IsType<UnprocessableEntityObjectResult>(result);
        //        var response = Assert.IsType<ApiResponse<object>>(unprocessableEntityResult.Value);
        //        Assert.Equal("Plan creation failed.", response.Message);
        //    }
        //}

        //[Fact]
        //public async Task CreateAsync_OperationCancelled_ReturnsBadRequest()
        //{
        //    // Arrange
        //    var createDto = new CreatePlanDto
        //    {
        //        Type = PlanTypes.Individual.ToString(),
        //        Status = PlanStatus.Active.ToString(),
        //        TotalFees = 100,
        //        StartDate = DateTime.Today,
        //        Days = new List<PlanDayDto>()
        //    };
        //    _planServiceMock.Setup(s => s.CreatePlanAsync(It.IsAny<CreatePlanDto>(), It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(Result.Failure<PlanDto>(CommonErrors.OperationCancelled));

        //    // Act
        //    var result = await _controller.CreateAsync(createDto, CancellationToken.None);

        //    // Assert
        //    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        //    var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
        //    Assert.False(response.IsSuccess);
        //    Assert.Equal("Operation cancelled.", response.Message);
        //}
        #endregion
    }
}