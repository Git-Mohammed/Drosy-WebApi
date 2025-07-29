using Drosy.Domain.Enums;
using System.Text.Json.Serialization;

namespace Drosy.Application.UseCases.Payments.DTOs
{
    public class PaymentDetailDTO
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentMethod Method { get; set; } // Add field if applicable
        public string? Notes { get; set; }
    }
}
