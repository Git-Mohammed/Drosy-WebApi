//using Drosy.Api.Controllers;
//using Drosy.Application.UseCases.Subjects.DTOs;
//using Drosy.Application.UseCases.Subjects.Interfaces;
//using Drosy.Domain.Shared.ApplicationResults;
//using Drosy.Api.Commons.Responses;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Xunit;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Drosy.Tests.Controllers.Subjects;

//public class SubjectsControllerTests
//{
//    private readonly Mock<ISubjectService> _mockService;
//    private readonly Mock<ILogger<SubjectsController>> _mockLogger;
//    private readonly SubjectsController _controller;

//    public SubjectsControllerTests()
//    {
//        _mockService = new Mock<ISubjectService>();
//        _mockLogger = new Mock<ILogger<SubjectsController>>();
//        _controller = new SubjectsController(_mockService.Object, _mockLogger.Object);
//    }

//    #region 🧪 GetByIdAsync Tests

//    [Theory]
//    [InlineData(0)]
//    [InlineData(-5)]
//    public async Task GetByIdAsync_InvalidId_ReturnsBadRequest(int invalidId)
//    {
//        // Act
//        var result = await _controller.GetByIdAsync(invalidId, CancellationToken.None);

//        // Assert
//        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
//        var response = Assert.IsType<ApiResponse<object>>(badRequest.Value);
//        Assert.Equal(400, response);
//        Assert.Contains("Invalid subject ID", response.Message);
//    }

//    [Fact]
//    public async Task GetByIdAsync_SubjectNotFound_ReturnsNotFound()
//    {
//        // Arrange
//        int subjectId = 99;
//        _mockService
//            .Setup(s => s.GetByIdAsync(subjectId, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(Result.Failure<SubjectDTO>("Subject not found"));

//        // Act
//        var result = await _controller.GetByIdAsync(subjectId, CancellationToken.None);

//        // Assert
//        var notFound = Assert.IsType<NotFoundObjectResult>(result);
//        var response = Assert.IsType<ApiResponse<object>>(notFound.Value);
//        Assert.Equal(404, response.Status);
//        Assert.Contains("Subject not found", response.Message);
//    }

//    [Fact]
//    public async Task GetByIdAsync_ValidId_ReturnsSuccess()
//    {
//        // Arrange
//        int subjectId = 1;
//        var subject = new SubjectDTO { Id = subjectId, Name = "Math" };

//        _mockService
//            .Setup(s => s.GetByIdAsync(subjectId, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(Result.Success(subject));

//        // Act
//        var result = await _controller.GetByIdAsync(subjectId, CancellationToken.None);

//        // Assert
//        var ok = Assert.IsType<OkObjectResult>(result);
//        var response = Assert.IsType<ApiResponse<SubjectDTO>>(ok.Value);
//        Assert.Equal(200, response.Status);
//        Assert.Equal("Math", response.Data.Name);
//    }

//    #endregion
//}
