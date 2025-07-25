namespace Drosy.Application.UseCases.Sessions.DTOs;

public class LessonStatsDto
{
    public int CompletedLessons { get; set; }   // حصص مكتملة
    public int UpcomingLessons { get; set; }    // حصص قادمة
    public int TotalLessons { get; set; }       // إجمالي الحصص
}