namespace Drosy.Application.UseCases.Payments.DTOs
{
    public class PaymentHistoryFilterDTO
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

}
