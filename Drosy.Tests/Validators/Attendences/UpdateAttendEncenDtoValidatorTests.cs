using Drosy.Application.UseCases.Attendences.DTOs;
using Drosy.Domain.Enums;
using Drosy.Infrastructure.Validators.AttendencesValidators;
using FluentValidation.TestHelper;

namespace Drosy.Tests.Validators.Attendences
{
    public class UpdateAttendEncenDtoValidatorTests
    {
        private readonly UpdateAttendencenDtoValidator _validator = new UpdateAttendencenDtoValidator();

        [Theory]
        [InlineData((AttendenceStatus)99, false)]
        [InlineData(AttendenceStatus.Absent, true)]
        public void StatusValidation(AttendenceStatus status, bool valid)
        {
            var model = new UpdateAttendencenDto { Status = status, Note = "x" };
            var result = _validator.TestValidate(model);
            if (valid) result.ShouldNotHaveValidationErrorFor(x => x.Status);
            else result.ShouldHaveValidationErrorFor(x => x.Status);
        }

        [Theory]
        [InlineData(null, true)]  // allow null note as valid
        [InlineData("", true)]
        [InlineData("ok", true)]
        public void NoteValidation(string note, bool valid)
        {
            var model = new UpdateAttendencenDto { Status = AttendenceStatus.Present, Note = note };
            var result = _validator.TestValidate(model);
            if (valid)
                result.ShouldNotHaveValidationErrorFor(x => x.Note);
            else
                result.ShouldHaveValidationErrorFor(x => x.Note);
        }

    }
}
