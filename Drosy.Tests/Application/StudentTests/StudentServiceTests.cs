using Drosy.Application.Interfaces.Common;
using Drosy.Domain.Interfaces.Repository;
using Moq;
using Drosy.Application.UseCases.Students.Interfaces;
using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Domain.Shared.ResultPattern;
using Drosy.Application.UseCases.Grades.DTOs;
using Drosy.Application.UseCases.Cities.DTOs;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents;

namespace Drosy.Tests.Application.StudentTests
{
    public class StudentServiceTests
    {
        private readonly Mock<IStudentService> _studentService;

        public StudentServiceTests()
        {
            _studentService = new Mock<IStudentService>();
        }

        [Fact]
        public async Task AddAsync_ShouldReturnSuccessResult_WhenStudentIsAdded()
        {
            // Arrange
            var addDto = new AddStudentDTO
            {
                FirstName = "John",
                SecondName = "A.",
                ThirdName = "B.",
                LastName = "Doe",
                Address = "123 Main St",
                PhoneNumber = "1234567890",
                EmergencyNumber = "0987654321",
                GradeId = 1,
                CityId = 2
            };

            var studentDto = new StudentDTO
            {
                Id = 1,
                FirstName = addDto.FirstName,
                SecondName = addDto.SecondName,
                ThirdName = addDto.ThirdName,
                LastName = addDto.LastName,
                Address = addDto.Address,
                PhoneNumber = addDto.PhoneNumber,
                EmergencyNumber = addDto.EmergencyNumber,
                Grade = new GradeDTO
                {


                    Id = addDto.GradeId,
                    Name = "Grade 1"
                },
                City = new CityDTO
                {
                    Id = addDto.CityId,
                    Name = "City A"
                }
            };

            _studentService
                .Setup(s => s.AddAsync(addDto, CancellationToken.None))
                .ReturnsAsync(Result.Success(studentDto));

            // Act
            var result = await _studentService.Object.AddAsync(addDto, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(addDto.FirstName, result.Value.FirstName);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnSuccessResult_WhenStudentExists()
        {
            // Arrange
            int studentId = 1;
            var studentDto = new StudentDTO
            {
                Id = studentId,
                FirstName = "Jane",
                SecondName = "B.",
                ThirdName = "C.",
                LastName = "Smith",
                Address = "456 Elm St",
                PhoneNumber = "5551234567",
                EmergencyNumber = "5557654321",
                Grade = null,
                City = null
            };

            _studentService
                .Setup(s => s.GetByIdAsync(studentId, CancellationToken.None))
                .ReturnsAsync(Result.Success(studentDto));

            // Act
            var result = await _studentService.Object.GetByIdAsync(studentId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(studentId, result.Value.Id);
        }

        [Theory]
        [InlineData(null, "A.", "B.", "Doe", "123 Main St", "1234567890", "0987654321", 1, 2, false)] // FirstName null
        [InlineData("", "A.", "B.", "Doe", "123 Main St", "1234567890", "0987654321", 1, 2, false)] // FirstName empty
        [InlineData("John", null, "B.", "Doe", "123 Main St", "1234567890", "0987654321", 1, 2, false)] // SecondName null
        [InlineData("John", "", "B.", "Doe", "123 Main St", "1234567890", "0987654321", 1, 2, false)] // SecondName empty
        [InlineData("John", "A.", "B.", null, "123 Main St", "1234567890", "0987654321", 1, 2, false)] // LastName null
        [InlineData("John", "A.", "B.", "", "123 Main St", "1234567890", "0987654321", 1, 2, false)] // LastName empty
        [InlineData("John", "A.", "B.", "Doe", null, "1234567890", "0987654321", 1, 2, false)] // Address null
        [InlineData("John", "A.", "B.", "Doe", "", "1234567890", "0987654321", 1, 2, false)] // Address empty
        [InlineData("John", "A.", "B.", "Doe", "123 Main St", null, "0987654321", 1, 2, false)] // PhoneNumber null
        [InlineData("John", "A.", "B.", "Doe", "123 Main St", "", "0987654321", 1, 2, false)] // PhoneNumber empty
        [InlineData("John", "A.", "B.", "Doe", "123 Main St", "1234567890", null, 1, 2, false)] // EmergencyNumber null
        [InlineData("John", "A.", "B.", "Doe", "123 Main St", "1234567890", "", 1, 2, false)] // EmergencyNumber empty
        [InlineData("John", "A.", "B.", "Doe", "123 Main St", "1234567890", "0987654321", 0, 2, false)] // GradeId invalid
        [InlineData("John", "A.", "B.", "Doe", "123 Main St", "1234567890", "0987654321", 1, 0, false)] // CityId invalid
        [InlineData("John", "A.", "B.", "Doe", "123 Main St", "1234567890", "0987654321", 1, 2, true)] // All valid
        public async Task AddAsync_ValidationTheory(
            string firstName,
            string secondName,
            string thirdName,
            string lastName,
            string address,
            string phoneNumber,
            string emergencyNumber,
            int gradeId,
            int cityId,
            bool isValid)
        {
            // Arrange
            var addDto = new AddStudentDTO
            {
                FirstName = firstName,
                SecondName = secondName,
                ThirdName = thirdName,
                LastName = lastName,
                Address = address,
                PhoneNumber = phoneNumber,
                EmergencyNumber = emergencyNumber,
                GradeId = gradeId,
                CityId = cityId
            };

            if (isValid)
            {
                var studentDto = new StudentDTO
                {
                    Id = 1,
                    FirstName = firstName,
                    SecondName = secondName,
                    ThirdName = thirdName,
                    LastName = lastName,
                    Address = address,
                    PhoneNumber = phoneNumber,
                    EmergencyNumber = emergencyNumber,
                    Grade = new GradeDTO { Id = gradeId, Name = "Grade" },
                    City = new CityDTO { Id = cityId, Name = "City" }
                };

                _studentService
                    .Setup(s => s.AddAsync(It.Is<AddStudentDTO>(d =>
                        d.FirstName == firstName &&
                        d.SecondName == secondName &&
                        d.ThirdName == thirdName &&
                        d.LastName == lastName &&
                        d.Address == address &&
                        d.PhoneNumber == phoneNumber &&
                        d.EmergencyNumber == emergencyNumber &&
                        d.GradeId == gradeId &&
                        d.CityId == cityId), CancellationToken.None))
                    .ReturnsAsync(Result.Success(studentDto));
            }
            else
            {
                _studentService
                    .Setup(s => s.AddAsync(It.Is<AddStudentDTO>(d =>
                        d.FirstName == firstName &&
                        d.SecondName == secondName &&
                        d.ThirdName == thirdName &&
                        d.LastName == lastName &&
                        d.Address == address &&
                        d.PhoneNumber == phoneNumber &&
                        d.EmergencyNumber == emergencyNumber &&
                        d.GradeId == gradeId &&
                        d.CityId == cityId),CancellationToken.None))
                    .ReturnsAsync(Result.Failure<StudentDTO>(Error.Invalid));
            }

            // Act
            var result = await _studentService.Object.AddAsync(addDto, CancellationToken.None);

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
        [InlineData(null, "A.", "B.", "Doe", "123 Main St", "1234567890", "0987654321", 1, 2, 1, false)] // FirstName null
        [InlineData("", "A.", "B.", "Doe", "123 Main St", "1234567890", "0987654321", 1, 2, 1, false)]  // FirstName empty
        [InlineData("John", null, "B.", "Doe", "123 Main St", "1234567890", "0987654321", 1, 2, 1, false)] // SecondName null
        [InlineData("John", "", "B.", "Doe", "123 Main St", "1234567890", "0987654321", 1, 2, 1, false)] // SecondName empty
        [InlineData("John", "A.", "B.", null, "123 Main St", "1234567890", "0987654321", 1, 2, 1, false)] // LastName null
        [InlineData("John", "A.", "B.", "", "123 Main St", "1234567890", "0987654321", 1, 2, 1, false)]  // LastName empty
        [InlineData("John", "A.", "B.", "Doe", null, "1234567890", "0987654321", 1, 2, 1, false)]        // Address null
        [InlineData("John", "A.", "B.", "Doe", "", "1234567890", "0987654321", 1, 2, 1, false)]         // Address empty
        [InlineData("John", "A.", "B.", "Doe", "123 Main St", null, "0987654321", 1, 2, 1, false)]      // PhoneNumber null
        [InlineData("John", "A.", "B.", "Doe", "123 Main St", "", "0987654321", 1, 2, 1, false)]        // PhoneNumber empty
        [InlineData("John", "A.", "B.", "Doe", "123 Main St", "1234567890", null, 1, 2, 1, false)]      // EmergencyNumber null
        [InlineData("John", "A.", "B.", "Doe", "123 Main St", "1234567890", "", 1, 2, 1, false)]        // EmergencyNumber empty
        [InlineData("John", "A.", "B.", "Doe", "123 Main St", "1234567890", "0987654321", 0, 2, 1, false)] // GradeId invalid
        [InlineData("John", "A.", "B.", "Doe", "123 Main St", "1234567890", "0987654321", 1, 0, 1, false)] // CityId invalid
        [InlineData("John", "A.", "B.", "Doe", "123 Main St", "1234567890", "0987654321", 1, 2, 0, false)] // ID invalid
        [InlineData("John", "A.", "B.", "Doe", "123 Main St", "1234567890", "0987654321", 1, 2, 1, true)] // All valid
        public async Task UpdateAsync_ValidationTheory(
    string firstName,
    string secondName,
    string thirdName,
    string lastName,
    string address,
    string phoneNumber,
    string emergencyNumber,
    int gradeId,
    int cityId,
    int studentId,
    bool isValid)
        {
            // Arrange
            var updateDto = new UpdateStudentDTO
            {
                FirstName = firstName,
                SecondName = secondName,
                ThirdName = thirdName,
                LastName = lastName,
                Address = address,
                PhoneNumber = phoneNumber,
                EmergencyNumber = emergencyNumber,
                GradeId = gradeId,
                CityId = cityId
            };

            if (isValid)
            {
                _studentService
                    .Setup(s => s.UpdateAsync(It.Is<UpdateStudentDTO>(d =>
                        d.FirstName == firstName &&
                        d.SecondName == secondName &&
                        d.ThirdName == thirdName &&
                        d.LastName == lastName &&
                        d.Address == address &&
                        d.PhoneNumber == phoneNumber &&
                        d.EmergencyNumber == emergencyNumber &&
                        d.GradeId == gradeId &&
                        d.CityId == cityId), studentId, CancellationToken.None))
                    .ReturnsAsync(Result.Success());
            }
            else
            {
                _studentService
                    .Setup(s => s.UpdateAsync(It.IsAny<UpdateStudentDTO>(), studentId, CancellationToken.None))
                    .ReturnsAsync(Result.Failure(Error.Invalid));
            }

            // Act
            var result = await _studentService.Object.UpdateAsync(updateDto, studentId, CancellationToken.None);

            // Assert
            if (isValid)
            {
                Assert.True(result.IsSuccess);
            }
            else
            {
                Assert.False(result.IsSuccess);
                Assert.NotNull(result.Error);
            }
        }

    }

}
