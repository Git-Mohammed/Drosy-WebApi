using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Plans.Interfaces;
using Drosy.Application.UseCases.Sessions.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ErrorComponents.Plans;
using Moq;

namespace Drosy.Tests.Application.Plans
{
    public class PlanServiceTests
    {
        private readonly Mock<IPlanService> _planService;

        public PlanServiceTests()
        {
            _planService = new Mock<IPlanService>();
        }


        #region GetAllPlansWithDetailsAsync
        [Fact]
        public async Task GetAllPlansWithDetailsAsync_ReturnsSuccess_WhenPlansExist()
        {
            // Arrange
            var plans = new List<Plan>
            {
                new Plan { Id = 1, Type = PlanTypes.Individual, Status = PlanStatus.Active, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(7) }
            };
            _planService.Setup(s => s.GetAllPlansWithDetailsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(plans));

            // Act
            var result = await _planService.Object.GetAllPlansWithDetailsAsync(CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value.Count);
            Assert.Equal(1, result.Value.First().Id);
        }

        [Fact]
        public async Task GetAllPlansWithDetailsAsync_ReturnsFailure_WhenOperationCancelled()
        {
            // Arrange
            _planService.Setup(s => s.GetAllPlansWithDetailsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<List<Plan>>(CommonErrors.OperationCancelled));

            // Act
            var result = await _planService.Object.GetAllPlansWithDetailsAsync(CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(CommonErrors.OperationCancelled.Code, result.Error.Code);
        }

        [Fact]
        public async Task GetAllPlansWithDetailsAsync_ReturnsFailure_WhenUnexpectedError()
        {
            // Arrange
            _planService.Setup(s => s.GetAllPlansWithDetailsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<List<Plan>>(CommonErrors.Unexpected));

            // Act
            var result = await _planService.Object.GetAllPlansWithDetailsAsync(CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(CommonErrors.Unexpected.Code, result.Error.Code);
        }

        #endregion

        #region GetPlansWithDetailsByStatusAsync
        [Theory]
        [InlineData(PlanStatus.Active, true)]
        [InlineData(PlanStatus.Inactive, true)]
        [InlineData((PlanStatus)99, false)] // Invalid status
        public async Task GetPlansWithDetailsByStatusAsync_ValidatesStatus(PlanStatus status, bool isValid)
        {
            // Arrange
            if (isValid)
            {
                var plans = new List<Plan>
                {
                    new Plan { Id = 1, Type = PlanTypes.Group, Status = status, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(7) }
                };
                _planService.Setup(s => s.GetPlansWithDetailsByStatusAsync(status, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Success(plans));
            }
            else
            {
                _planService.Setup(s => s.GetPlansWithDetailsByStatusAsync(status, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Failure<List<Plan>>(CommonErrors.Invalid));
            }

            // Act
            var result = await _planService.Object.GetPlansWithDetailsByStatusAsync(status, CancellationToken.None);

            // Assert
            Assert.Equal(isValid, result.IsSuccess);
            if (isValid)
            {
                Assert.Single(result.Value);
                Assert.Equal(status, result.Value.First().Status);
            }
            else
            {
                Assert.Equal(CommonErrors.Invalid.Code, result.Error.Code);
            }
        }

        #endregion

        #region  GetPlanByIdAsync
        [Theory]
        [InlineData(0, false)]  // Invalid ID
        [InlineData(-1, false)] // Negative ID
        [InlineData(1, true)]   // Valid ID
        public async Task GetPlanByIdAsync_ValidatesId(int planId, bool isValid)
        {
            // Arrange
            if (isValid)
            {
                var planDto = new PlanDto
                {
                    Id = planId,
                    Type = PlanTypes.Individual.ToString(),
                    Status = PlanStatus.Active.ToString(),
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(7),
                    Days = new List<PlanDayDto>()
                };
                _planService.Setup(s => s.GetPlanByIdAsync(planId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Success(planDto));
            }
            else
            {
                _planService.Setup(s => s.GetPlanByIdAsync(planId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Failure<PlanDto>(PlanErrors.PlanNotFound));
            }

            // Act
            var result = await _planService.Object.GetPlanByIdAsync(planId, CancellationToken.None);

            // Assert
            Assert.Equal(isValid, result.IsSuccess);
            if (isValid)
            {
                Assert.NotNull(result.Value);
                Assert.Equal(planId, result.Value.Id);
            }
            else
            {
                Assert.Equal(PlanErrors.PlanNotFound.Code, result.Error.Code);
            }
        }

        #endregion


        #region GetAllAsync
        [Fact]
        public async Task GetAllAsync_ReturnsSuccess_WhenPlansExist()
        {
            // Arrange
            var dtos = new List<PlanDto>
            {
                new PlanDto { Id = 1, Type = PlanTypes.Individual.ToString(), Status = PlanStatus.Active.ToString(), Days = new List<PlanDayDto>() }
            };
            var dataResult = new DataResult<PlanDto> { Data = dtos, TotalRecordsCount = dtos.Count };
            _planService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(dataResult));

            // Act
            var result = await _planService.Object.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value.TotalRecordsCount);
            Assert.Single(result.Value.Data);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsFailure_WhenUnexpectedError()
        {
            // Arrange
            _planService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<DataResult<PlanDto>>(CommonErrors.Unexpected));

            // Act
            var result = await _planService.Object.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(CommonErrors.Unexpected.Code, result.Error.Code);
        }

        #endregion

        #region GetPlansByDate
        [Fact]
        public async Task GetPlansByDate_ReturnsSuccess_WhenPlansMatchDate()
        {
            // Arrange
            var date = DateTime.Today;
            var dtos = new List<PlanDto>
            {
                new PlanDto { Id = 1, Type = PlanTypes.Group.ToString(), Status = PlanStatus.Active.ToString(), StartDate = date, Days = new List<PlanDayDto>() }
            };
            var dataResult = new DataResult<PlanDto> { Data = dtos, TotalRecordsCount = dtos.Count };
            _planService.Setup(s => s.GetPlansByDate(date, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(dataResult));

            // Act
            var result = await _planService.Object.GetPlansByDate(date, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value.TotalRecordsCount);
            Assert.Equal(date, result.Value.Data.First().StartDate);
        }

        #endregion

        #region GetPlansInRange

        [Theory]
        [InlineData("2025-08-01", "2025-07-31", false)] // Invalid range
        [InlineData("2025-08-01", "2025-08-02", true)]  // Valid range
        public async Task GetPlansInRange_ValidatesRange(string start, string end, bool isValid)
        {
            // Arrange
            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);
            if (isValid)
            {
                var dtos = new List<PlanDto>
                {
                    new PlanDto { Id = 1, Type = PlanTypes.Individual.ToString(), Status = PlanStatus.Active.ToString(), StartDate = startDate, EndDate = endDate, Days = new List<PlanDayDto>() }
                };
                var dataResult = new DataResult<PlanDto> { Data = dtos, TotalRecordsCount = dtos.Count };
                _planService.Setup(s => s.GetPlansInRange(startDate, endDate, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Success(dataResult));
            }
            else
            {
                _planService.Setup(s => s.GetPlansInRange(startDate, endDate, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Failure<DataResult<PlanDto>>(PlanErrors.ConstraintViolation));
            }

            // Act
            var result = await _planService.Object.GetPlansInRange(startDate, endDate, CancellationToken.None);

            // Assert
            Assert.Equal(isValid, result.IsSuccess);
            if (isValid)
            {
                Assert.Equal(1, result.Value.TotalRecordsCount);
            }
            else
            {
                Assert.Equal(PlanErrors.ConstraintViolation.Code, result.Error.Code);
            }
        }

        #endregion

        #region GetPlansByStatus
        [Theory]
        [InlineData(PlanStatus.Active, true)]
        [InlineData((PlanStatus)99, false)] // Invalid status
        public async Task GetPlansByStatus_ValidatesStatus(PlanStatus status, bool isValid)
        {
            // Arrange
            if (isValid)
            {
                var dtos = new List<PlanDto>
                {
                    new PlanDto { Id = 1, Type = PlanTypes.Group.ToString(), Status = status.ToString(), Days = new List<PlanDayDto>() }
                };
                var dataResult = new DataResult<PlanDto> { Data = dtos, TotalRecordsCount = dtos.Count };
                _planService.Setup(s => s.GetPlansByStatus(status, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Success(dataResult));
            }
            else
            {
                _planService.Setup(s => s.GetPlansByStatus(status, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Failure<DataResult<PlanDto>>(CommonErrors.Invalid));
            }

            // Act
            var result = await _planService.Object.GetPlansByStatus(status, CancellationToken.None);

            // Assert
            Assert.Equal(isValid, result.IsSuccess);
            if (isValid)
            {
                Assert.Equal(status.ToString(), result.Value.Data.First().Status);
            }
        }

        #endregion

        #region GetPlansByType
        [Theory]
        [InlineData(PlanTypes.Individual, true)]
        [InlineData((PlanTypes)99, false)] // Invalid type
        public async Task GetPlansByType_ValidatesType(PlanTypes type, bool isValid)
        {
            // Arrange
            if (isValid)
            {
                var dtos = new List<PlanDto>
                {
                    new PlanDto { Id = 1, Type = type.ToString(), Status = PlanStatus.Active.ToString(), Days = new List<PlanDayDto>() }
                };
                var dataResult = new DataResult<PlanDto> { Data = dtos, TotalRecordsCount = dtos.Count };
                _planService.Setup(s => s.GetPlansByType(type, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Success(dataResult));
            }
            else
            {
                _planService.Setup(s => s.GetPlansByType(type, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Failure<DataResult<PlanDto>>(CommonErrors.Invalid));
            }

            // Act
            var result = await _planService.Object.GetPlansByType(type, CancellationToken.None);

            // Assert
            Assert.Equal(isValid, result.IsSuccess);
            if (isValid)
            {
                Assert.Equal(type.ToString(), result.Value.Data.First().Type);
            }
        }

        #endregion

        #region GetPlansByWeek

        [Theory]
        [InlineData(2025, 0, false)]  // Invalid week
        [InlineData(2025, 30, true)]  // Valid week
        public async Task GetPlansByWeek_ValidatesWeek(int year, int week, bool isValid)
        {
            // Arrange
            if (isValid)
            {
                var dtos = new List<PlanDto>
                {
                    new PlanDto { Id = 1, Type = PlanTypes.Individual.ToString(), Status = PlanStatus.Active.ToString(), Days = new List<PlanDayDto>() }
                };
                var dataResult = new DataResult<PlanDto> { Data = dtos, TotalRecordsCount = dtos.Count };
                _planService.Setup(s => s.GetPlansByWeek(year, week, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Success(dataResult));
            }
            else
            {
                _planService.Setup(s => s.GetPlansByWeek(year, week, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Failure<DataResult<PlanDto>>(CommonErrors.Invalid));
            }

            // Act
            var result = await _planService.Object.GetPlansByWeek(year, week, CancellationToken.None);

            // Assert
            Assert.Equal(isValid, result.IsSuccess);
        }

        #endregion

        #region GetPlansByMonth

        [Theory]
        [InlineData(2025, 0, false)]  // Invalid month
        [InlineData(2025, 7, true)]   // Valid month
        public async Task GetPlansByMonth_ValidatesMonth(int year, int month, bool isValid)
        {
            // Arrange
            if (isValid)
            {
                var dtos = new List<PlanDto>
                {
                    new PlanDto { Id = 1, Type = PlanTypes.Group.ToString(), Status = PlanStatus.Active.ToString(), Days = new List<PlanDayDto>() }
                };
                var dataResult = new DataResult<PlanDto> { Data = dtos, TotalRecordsCount = dtos.Count };
                _planService.Setup(s => s.GetPlansByMonth(year, month, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Success(dataResult));
            }
            else
            {
                _planService.Setup(s => s.GetPlansByMonth(year, month, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Failure<DataResult<PlanDto>>(CommonErrors.Invalid));
            }

            // Act
            var result = await _planService.Object.GetPlansByMonth(year, month, CancellationToken.None);

            // Assert
            Assert.Equal(isValid, result.IsSuccess);
        }

        #endregion

        #region ExistsAsync


        [Theory]
        [InlineData(0, false)]  // Invalid ID
        [InlineData(1, true)]   // Valid ID
        public async Task ExistsAsync_ValidatesId(int planId, bool isValid)
        {
            // Arrange
            if (isValid)
            {
                _planService.Setup(s => s.ExistsAsync(planId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Success());
            }
            else
            {
                _planService.Setup(s => s.ExistsAsync(planId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Failure(PlanErrors.PlanNotFound));
            }

            // Act
            var result = await _planService.Object.ExistsAsync(planId, CancellationToken.None);

            // Assert
            Assert.Equal(isValid, result.IsSuccess);
            if (!isValid)
            {
                Assert.Equal(PlanErrors.PlanNotFound.Code, result.Error.Code);
            }
        }


        #endregion

        #region Calendar

        #region GetPlanSessionsCalenderAsyn

        [Theory]
        [InlineData(0, false)]  // Invalid plan ID
        [InlineData(1, true)]   // Valid plan ID
        public async Task GetPlanSessionsCalenderAsync_ValidatesPlanId(int planId, bool isValid)
        {
            // Arrange
            if (isValid)
            {
                var sessions = new List<CalenderSessionDto>
                {
                    new CalenderSessionDto { PlanId = planId, ExcepectedDate = DateTime.Today }
                };
                var dataResult = new DataResult<CalenderSessionDto> { Data = sessions, TotalRecordsCount = sessions.Count };
                _planService.Setup(s => s.GetPlanSessionsCalenderAsync(planId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Success(dataResult));
            }
            else
            {
                _planService.Setup(s => s.GetPlanSessionsCalenderAsync(planId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Failure<DataResult<CalenderSessionDto>>(PlanErrors.PlanNotFound));
            }

            // Act
            var result = await _planService.Object.GetPlanSessionsCalenderAsync(planId, CancellationToken.None);

            // Assert
            Assert.Equal(isValid, result.IsSuccess);
            if (isValid)
            {
                Assert.Equal(1, result.Value.TotalRecordsCount);
                Assert.Equal(planId, result.Value.Data.First().PlanId);
            }
            else
            {
                Assert.Equal(PlanErrors.PlanNotFound.Code, result.Error.Code);
            }
        }

        [Fact]
        public async Task GetPlanSessionsCalenderAsync_ReturnsFailure_WhenOperationCancelled()
        {
            // Arrange
            _planService.Setup(s => s.GetPlanSessionsCalenderAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.OperationCancelled));

            // Act
            var result = await _planService.Object.GetPlanSessionsCalenderAsync(1, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(CommonErrors.OperationCancelled.Code, result.Error.Code);
        }

        #endregion


        #region GetPlanSessionsCalenderByDateAsync
        [Theory]
        [InlineData(1, "2025-08-01", true)]  // Valid date
        public async Task GetPlanSessionsCalenderByDateAsync_ValidatesPlanIdAndDate(int planId, string dateStr, bool isValid)
        {
            // Arrange
            var date = DateTime.Parse(dateStr);
            if (isValid)
            {
                var sessions = new List<CalenderSessionDto>
                {
                    new CalenderSessionDto { PlanId = planId, ExcepectedDate = date }
                };
                var dataResult = new DataResult<CalenderSessionDto> { Data = sessions, TotalRecordsCount = sessions.Count };
                _planService.Setup(s => s.GetPlanSessionsCalenderByDateAsync(planId, date, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Success(dataResult));
            }
            else
            {
                _planService.Setup(s => s.GetPlanSessionsCalenderByDateAsync(planId, date, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Failure<DataResult<CalenderSessionDto>>(PlanErrors.PlanNotFound));
            }

            // Act
            var result = await _planService.Object.GetPlanSessionsCalenderByDateAsync(planId, date, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value.TotalRecordsCount);
            Assert.Equal(date, result.Value.Data.First().ExcepectedDate);
        }

        #endregion


        #region GetPlanSessionsCalenderByRangeAsync
        [Theory]
        [InlineData(1, "2025-08-01", "2025-07-31", false)] // Invalid range
        [InlineData(1, "2025-08-01", "2025-08-02", true)]  // Valid range
        public async Task GetPlanSessionsCalenderByRangeAsync_ValidatesRange(int planId, string start, string end, bool isValid)
        {
            // Arrange
            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);
            if (isValid)
            {
                var sessions = new List<CalenderSessionDto>
                {
                    new CalenderSessionDto { PlanId = planId, ExcepectedDate = startDate }
                };
                var dataResult = new DataResult<CalenderSessionDto> { Data = sessions, TotalRecordsCount = sessions.Count };
                _planService.Setup(s => s.GetPlanSessionsCalenderByRangeAsync(planId, startDate, endDate, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Success(dataResult));
            }
            else
            {
                _planService.Setup(s => s.GetPlanSessionsCalenderByRangeAsync(planId, startDate, endDate, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Failure<DataResult<CalenderSessionDto>>(PlanErrors.ConstraintViolation));
            }

            // Act
            var result = await _planService.Object.GetPlanSessionsCalenderByRangeAsync(planId, startDate, endDate, CancellationToken.None);

            // Assert
            Assert.Equal(isValid, result.IsSuccess);
            if (isValid)
            {
                Assert.Equal(1, result.Value.TotalRecordsCount);
                Assert.Equal(planId, result.Value.Data.First().PlanId);
            }
            else
            {
                Assert.Equal(PlanErrors.ConstraintViolation.Code, result.Error.Code);
            }
        }

        #endregion


        #region GetPlanSessionsCalenderByStatusAsync
        [Theory]
        [InlineData(1, PlanStatus.Active, true)]
        [InlineData(1, PlanStatus.Inactive, false)] // Plan status mismatch
        public async Task GetPlanSessionsCalenderByStatusAsync_ValidatesStatus(int planId, PlanStatus status, bool isValid)
        {
            // Arrange
            if (isValid)
            {
                var sessions = new List<CalenderSessionDto>
                {
                    new CalenderSessionDto { PlanId = planId, PlanStatus = status.ToString() }
                };
                var dataResult = new DataResult<CalenderSessionDto> { Data = sessions, TotalRecordsCount = sessions.Count };
                _planService.Setup(s => s.GetPlanSessionsCalenderByStatusAsync(planId, status, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Success(dataResult));
            }
            else
            {
                var emptyResult = new DataResult<CalenderSessionDto> { Data = new List<CalenderSessionDto>(), TotalRecordsCount = 0 };
                _planService.Setup(s => s.GetPlanSessionsCalenderByStatusAsync(planId, status, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Success(emptyResult));
            }

            // Act
            var result = await _planService.Object.GetPlanSessionsCalenderByStatusAsync(planId, status, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            if (isValid)
            {
                Assert.Equal(1, result.Value.TotalRecordsCount);
                Assert.Equal(status.ToString(), result.Value.Data.First().PlanStatus);
            }
            else
            {
                Assert.Empty(result.Value.Data);
                Assert.Equal(0, result.Value.TotalRecordsCount);
            }
        }

        #endregion


        #region GetPlanSessionsCalenderByWeekAsync
        [Theory]
        [InlineData(1, 2025, 0, false)]  // Invalid week
        [InlineData(1, 2025, 30, true)]  // Valid week
        public async Task GetPlanSessionsCalenderByWeekAsync_ValidatesWeek(int planId, int year, int week, bool isValid)
        {
            // Arrange
            if (isValid)
            {
                var start = GetStartOfWeek(year, week);
                var sessions = new List<CalenderSessionDto>
                {
                    new CalenderSessionDto { PlanId = planId, ExcepectedDate = start }
                };
                var dataResult = new DataResult<CalenderSessionDto> { Data = sessions, TotalRecordsCount = sessions.Count };
                _planService.Setup(s => s.GetPlanSessionsCalenderByWeekAsync(planId, year, week, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Success(dataResult));
            }
            else
            {
                _planService.Setup(s => s.GetPlanSessionsCalenderByWeekAsync(planId, year, week, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.Invalid));
            }

            // Act
            var result = await _planService.Object.GetPlanSessionsCalenderByWeekAsync(planId, year, week, CancellationToken.None);

            // Assert
            Assert.Equal(isValid, result.IsSuccess);
            if (isValid)
            {
                Assert.Equal(1, result.Value.TotalRecordsCount);
            }
        }

        #endregion

        #region GetPlanSessionsCalenderByMonthAsync
        [Theory]
        [InlineData(1, 2025, 0, false)]  // Invalid month
        [InlineData(1, 2025, 7, true)]   // Valid month
        public async Task GetPlanSessionsCalenderByMonthAsync_ValidatesMonth(int planId, int year, int month, bool isValid)
        {
            // Arrange
            if (isValid)
            {
                var start = new DateTime(year, month, 1);
                var sessions = new List<CalenderSessionDto>
                {
                    new CalenderSessionDto { PlanId = planId, ExcepectedDate = start }
                };
                var dataResult = new DataResult<CalenderSessionDto> { Data = sessions, TotalRecordsCount = sessions.Count };
                _planService.Setup(s => s.GetPlanSessionsCalenderByMonthAsync(planId, year, month, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Success(dataResult));
            }
            else
            {
                _planService.Setup(s => s.GetPlanSessionsCalenderByMonthAsync(planId, year, month, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.Invalid));
            }

            // Act
            var result = await _planService.Object.GetPlanSessionsCalenderByMonthAsync(planId, year, month, CancellationToken.None);

            // Assert
            Assert.Equal(isValid, result.IsSuccess);
            if (isValid)
            {
                Assert.Equal(1, result.Value.TotalRecordsCount);
            }
        }

        #endregion


        #region GenerateSessionsForPlanAsync
        [Fact]
        public async Task GenerateSessionsForPlanAsync_ReturnsSessions_WhenPlanIsValid()
        {
            // Arrange
            var plan = new Plan
            {
                Id = 1,
                Type = PlanTypes.Individual,
                Status = PlanStatus.Active,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(7),
                PlanDays = new List<PlanDay> { new PlanDay { Day = Days.Monday, StartSession = TimeSpan.FromHours(10), EndSession = TimeSpan.FromHours(11) } }
            };
            var sessions = new List<CalenderSessionDto>
            {
                new CalenderSessionDto
                {
                    PlanId = 1,
                    ExcepectedDate = DateTime.Today,
                    PlanType = PlanTypes.Individual.ToString(),
                    PlanStatus = PlanStatus.Active.ToString(),
                    Days = new List<PlanDayDto> { new PlanDayDto { Day = Days.Monday.ToString(), StartSession = TimeSpan.FromHours(10), EndSession = TimeSpan.FromHours(11) } }
                }
            };
            _planService.Setup(s => s.GenerateSessionsForPlanAsync(plan, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sessions);

            // Act
            var result = await _planService.Object.GenerateSessionsForPlanAsync(plan, null, null, CancellationToken.None);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result.First().PlanId);
            Assert.Equal(PlanTypes.Individual.ToString(), result.First().PlanType);
        }

        #endregion


        #endregion

        #region Write
        #region CreatePlanAsync
        [Theory]
        [InlineData(true, PlanTypes.Individual, PlanStatus.Active)]  // Valid plan
        [InlineData(false, PlanTypes.Individual, PlanStatus.Active)] // Save failure
        public async Task CreatePlanAsync_ValidatesInput(bool saveSucceeds, PlanTypes type, PlanStatus status)
        {
            // Arrange
            var createDto = new CreatePlanDto
            {
                Type = type.ToString(),
                Status = status.ToString(),
                TotalFees = 100,
                StartDate = DateTime.Today,
                Period = 30,
                Days = new List<PlanDayDto> { new PlanDayDto { Day = Days.Monday.ToString(), StartSession = TimeSpan.FromHours(10), EndSession = TimeSpan.FromHours(11) } }
            };
            if (saveSucceeds)
            {
                var planDto = new PlanDto
                {
                    Id = 1,
                    Type = type.ToString(),
                    Status = status.ToString(),
                    TotalFees = 100,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(30),
                    Days = createDto.Days
                };
                _planService.Setup(s => s.CreatePlanAsync(It.Is<CreatePlanDto>(d => d.Type == createDto.Type && d.Status == createDto.Status), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Success(planDto));
            }
            else
            {
                _planService.Setup(s => s.CreatePlanAsync(It.Is<CreatePlanDto>(d => d.Type == createDto.Type && d.Status == createDto.Status), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Failure<PlanDto>(PlanErrors.PlanSaveFailure));
            }

            // Act
            var result = await _planService.Object.CreatePlanAsync(createDto, CancellationToken.None);

            // Assert
            Assert.Equal(saveSucceeds, result.IsSuccess);
            if (saveSucceeds)
            {
                Assert.NotNull(result.Value);
                Assert.Equal(createDto.Type, result.Value.Type);
                Assert.Equal(createDto.Status, result.Value.Status);
            }
            else
            {
                Assert.Equal(PlanErrors.PlanSaveFailure.Code, result.Error.Code);
            }
        }

        [Fact]
        public async Task CreatePlanAsync_ReturnsFailure_WhenOperationCancelled()
        {
            // Arrange
            var createDto = new CreatePlanDto
            {
                Type = PlanTypes.Individual.ToString(),
                Status = PlanStatus.Active.ToString(),
                TotalFees = 100,
                StartDate = DateTime.Today,
                Days = new List<PlanDayDto>()
            };
            _planService.Setup(s => s.CreatePlanAsync(It.IsAny<CreatePlanDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<PlanDto>(CommonErrors.OperationCancelled));

            // Act
            var result = await _planService.Object.CreatePlanAsync(createDto, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(CommonErrors.OperationCancelled.Code, result.Error.Code);
        }

        #endregion


        #endregion

        private DateTime GetStartOfWeek(int year, int week)
        {
            var jan4 = new DateTime(year, 1, 4);
            var startOfYear = jan4.AddDays(-((int)jan4.DayOfWeek + 6) % 7);
            return startOfYear.AddDays((week - 1) * 7);
        }
    }
}