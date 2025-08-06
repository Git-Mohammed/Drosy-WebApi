namespace Drosy.Application.UseCases.Payments.DTOs
{
    public class StudentPaymentHistoryDTO
    {
        public int StudentId { get; set; }
        public List<PaymentDetailDTO> Payments { get; set; } = new();
        public decimal TotalPaid { get; set; }
        public decimal RemainingBalance { get; set; }
    }
}
