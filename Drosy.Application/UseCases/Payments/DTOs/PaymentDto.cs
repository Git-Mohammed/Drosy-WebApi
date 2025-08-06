using Drosy.Domain.Enums;
using System.Text.Json.Serialization;

namespace Drosy.Application.UseCases.Payments.DTOs;
public class PaymentDto
{
    public int Id { get; set; }
    public int PlanId { get; set; }
    public int StudentId { get; set; }
    public decimal Amount { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentMethod Method { get; set; }
    public string? Notes { get; set; }
    public DateTime PaymentDate { get; set; }
}
