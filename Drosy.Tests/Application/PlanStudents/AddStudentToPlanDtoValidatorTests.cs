using Drosy.Application.UseCases.PlanStudents.DTOs;
using Drosy.Infrastructure.Validators.PlanStudentsValidator;
using FluentValidation.TestHelper;

namespace Drosy.Tests.Application.PlanStudents
{
    public class AddStudentToPlanDtoValidatorTests
    {
        private readonly AddStudentToPlanDtoValidator _validator;

        public AddStudentToPlanDtoValidatorTests()
        {
            _validator = new AddStudentToPlanDtoValidator();
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(-1, false)]
        [InlineData(1, true)]
        public void Validator_ShouldHaveError_WhenStudentIdInvalid(int studentId, bool valid)
        {
            var model = new AddStudentToPlanDto { StudentId = studentId };
            var result = _validator.TestValidate(model);
            if (!valid)
                result.ShouldHaveValidationErrorFor(x => x.StudentId);
            else
                result.ShouldNotHaveValidationErrorFor(x => x.StudentId);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenNotesTooLong()
        {
            var notes = new string('a', 501);
            var model = new AddStudentToPlanDto { StudentId = 1, Notes = notes };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Notes);
        }

        [Theory]
        [InlineData(-0.01, false)]
        [InlineData(0, true)]
        [InlineData(10.5, true)]
        public void Validator_ShouldHaveError_WhenFeeInvalid(decimal fee, bool valid)
        {
            var model = new AddStudentToPlanDto { StudentId = 1, Fee = fee };
            var result = _validator.TestValidate(model);
            if (!valid)
                result.ShouldHaveValidationErrorFor(x => x.Fee);
            else
                result.ShouldNotHaveValidationErrorFor(x => x.Fee);
        }
    }
}
