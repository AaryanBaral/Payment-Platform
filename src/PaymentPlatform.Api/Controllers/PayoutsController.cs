using Microsoft.AspNetCore.Mvc;
using PaymentPlatform.Api.Models.Payouts;
using PaymentPlatform.Application.Common;
using PaymentPlatform.Application.Payouts.Commands.GeneratePayoutRequest;

namespace PaymentPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayoutsController : ControllerBase
    {
        private readonly GeneratePayoutRequestHandler _generatePayoutRequestHandler;

        public PayoutsController(GeneratePayoutRequestHandler generatePayoutRequestHandler)
        {
            _generatePayoutRequestHandler = generatePayoutRequestHandler;
        }

        [HttpPost("request")]
        public async Task<IActionResult> GeneratePayoutAsync(
            [FromBody] GeneratePayoutRequestDto dto,
            CancellationToken cancellationToken)
        {
            // 1. Map DTO → Command
            var command = new GeneratePayoutRequestCommand(
                dto.TenantId,
                dto.MerchantId,
                dto.RequestedAmount,
                dto.Currency,
                dto.RequestedByUserId);

            // 2. Call handler
            Result<GeneratePayoutRequestResult> result =
                await _generatePayoutRequestHandler.HandleAsync(command, cancellationToken);

            // 3. Map Result → HTTP response
            if (!result.IsSuccess)
            {
                // For now, simple 400 with error message. Later you can map to ProblemDetails.
                return BadRequest(new
                {
                    error = result.Error
                });
            }

            var payload = result.Value!;

            var response = new GeneratePayoutResponseDto
            {
                PayoutId = payload.PayoutId,
                TenantId = payload.TenantId,
                MerchantId = payload.MerchantId,
                Amount = payload.Amount,
                Currency = payload.Currency,
                Status = payload.Status
            };

            return Ok(response);
        }
    }
}