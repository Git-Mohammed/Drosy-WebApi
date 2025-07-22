using Drosy.Domain.Enums;

namespace Drosy.Application.UseCases.Attendences.DTOs
{
    public class AddAttendencenDto()
    {
        public int StudentId { get; set; }
        public AttendenceStatus Status { get; set; }
        public string Note { get; set; } = null!;
    }
}

