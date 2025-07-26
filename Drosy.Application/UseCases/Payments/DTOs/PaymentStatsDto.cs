namespace Drosy.Application.UseCases.Payments.DTOs;

public class PaymentStatsDto
{
    public decimal PaidAmount { get; set; }     // إجمالي المدفوع
    public decimal DueAmount { get; set; }      // المبلغ المستحق
    public decimal TotalAmount { get; set; }    // إجمالي القيمة
}