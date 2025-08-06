namespace Drosy.Application.UseCases.Students.DTOs;
public class StudentCardInfoDTO
{
    public string FullName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Grade { get; set; } = null!;
    public int PlansCount { get; set; }
    public int SessionsCount { get; set; }
}