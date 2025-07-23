using Drosy.Application.UseCases.Payments.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    [HttpPost]
    public IActionResult CrateNewPaymentAsync([FromBody] CreatePaymentDto payment, CancellationToken ct)
    {
        return Ok();
    }
    
}