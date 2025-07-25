namespace Drosy.Application.UseCases.Sessions.DTOs;

public class RescheduleSessionDTO
{
    public DateTime NewDate { get; set; }
    public DateTime NewStartTime { get; set; }
    public DateTime NewEndTime { get; set; }
}

