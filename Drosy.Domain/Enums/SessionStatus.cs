namespace Drosy.Domain.Enums;

public enum SessionStatus : byte
{
    Scheduled = 0,
    Completed = 1,
    Canceled,
    rescheduled,
}
