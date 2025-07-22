using Drosy.Domain.Enums;

namespace Drosy.Application.UseCases.Attendences.DTOs
{
    public class UpdateAttendencenDto()
    {
        public AttendenceStatus Status { get; set; }
        public string Note { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
    }
}

