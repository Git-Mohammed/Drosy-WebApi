namespace Drosy.Application.UseCases.Payments.DTOs;
public class CreatePaymentDto
{
    public int PlanId { get; set; }
    public int StudentId { get; set; }
    public decimal Amount { get; set; }
}