using Drosy.Application.UseCases.Attendences.DTOs;
using Drosy.Domain.Enums;
using Drosy.Infrastructure.Validators.AttendencesValidators;
using FluentValidation.TestHelper;

namespace Drosy.Tests.Application.Attendeces.ValidatorsTests
{
    public class AddAttendEncenDtoValidatorTests
    {
        private readonly AddAttendencenDtoValidator _validator = new AddAttendencenDtoValidator();

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void StudentIdValidation(int studentId, bool valid)
        {
            var model = new AddAttendencenDto { StudentId = studentId, Status = AttendenceStatus.Present, Note = "x" };
            var result = _validator.TestValidate(model);
            if (valid)
                result.ShouldNotHaveValidationErrorFor(x => x.StudentId);
            else
                result.ShouldHaveValidationErrorFor(x => x.StudentId);
        }

        [Theory]
        [InlineData(null, true)] 
        [InlineData("", true)]
        [InlineData("note", true)]
        public void NoteValidation(string note, bool valid)
        {
            var model = new AddAttendencenDto { StudentId = 1, Status = AttendenceStatus.Present, Note = note };
            var result = _validator.TestValidate(model);
            if (valid)
                result.ShouldNotHaveValidationErrorFor(x => x.Note);
            else
                result.ShouldHaveValidationErrorFor(x => x.Note);
        }

        [Theory]
        [InlineData((AttendenceStatus)99, false)]
        [InlineData(AttendenceStatus.Present, true)]
        public void StatusValidation(AttendenceStatus status, bool valid)
        {
            var model = new AddAttendencenDto { StudentId = 1, Status = status, Note = "x" };
            var result = _validator.TestValidate(model);
            if (valid)
                result.ShouldNotHaveValidationErrorFor(x => x.Status);
            else
                result.ShouldHaveValidationErrorFor(x => x.Status);
        }
    }
}
