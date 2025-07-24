using Drosy.Application.UseCases.Attendences.DTOs;
using Drosy.Domain.Enums;
using FluentValidation;
using System;

/// <summary>
/// Validator for <see cref="AddAttendencenDto"/> used to validate attendance data before processing.
/// </summary>
public class AddAttendencenDtoValidator : AbstractValidator<AddAttendencenDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddAttendencenDtoValidator"/> class
    /// and defines validation rules for AddAttendencenDto properties.
    /// </summary>
    public AddAttendencenDtoValidator()
    {
        /// <summary>
        /// Validates that <see cref="AddAttendencenDto.StudentId"/> is greater than zero.
        /// </summary>
        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("Please specify a valid student id.");

        /// <summary>
        /// Validates that <see cref="AddAttendencenDto.Note"/> does not exceed 500 characters.
        /// The note is optional (nullable), so no null check is enforced here.
        /// </summary>
        RuleFor(x => x.Note)
            .MaximumLength(500)
            .WithMessage("Note cannot exceed 500 characters.");

        /// <summary>
        /// Validates that <see cref="AddAttendencenDto.Status"/> is a valid value defined in the <see cref="AttendenceStatus"/> enum.
        /// </summary>
        RuleFor(x => x.Status)
            .Must(status => Enum.IsDefined(typeof(AttendenceStatus), status))
            .WithMessage("Invalid attendance status. Allowed values are: Present, Absent.");
    }
}
