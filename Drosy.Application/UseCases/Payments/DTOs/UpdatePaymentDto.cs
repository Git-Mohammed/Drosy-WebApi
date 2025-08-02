using System.Text.Json.Serialization;
using Drosy.Domain.Enums;

namespace Drosy.Application.UseCases.Payments.DTOs;

public class UpdatePaymentDto
{
    public decimal Amount { get; set; }
    public int StudentId { get; set; }
    public int PlanId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentMethod Method { get; set; }
    public string? Notes { get; set; }
}