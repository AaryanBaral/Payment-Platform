using Microsoft.AspNetCore.Mvc;
using PaymentPlatform.Api.Models.Payouts;
using PaymentPlatform.Application.Common;
using PaymentPlatform.Application.Payouts.Commands.GeneratePayoutRequest;
using PaymentPlatform.Application.Payouts.Commands.RejectPayout;

namespace PaymentPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayoutsController : ControllerBase
    {
        private readonly GeneratePayoutRequestHandler _generatePayoutRequestHandler;
        private readonly RejectPayoutHandler _rejectPayoutHandler;


        public PayoutsController(GeneratePayoutRequestHandler generatePayoutRequestHandler, RejectPayoutHandler rejectPayoutHandler)
        {
            _generatePayoutRequestHandler = generatePayoutRequestHandler;
            _rejectPayoutHandler = rejectPayoutHandler;
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

        [HttpPost("{payoutId:guid}/reject")]
        public async Task<IActionResult> RejectAsync(
    Guid payoutId,
    [FromBody] RejectPayoutRequestDto dto,
    CancellationToken cancellationToken)
        {
            var command = new RejectPayoutCommand(
                dto.TenantId,
                payoutId,
                dto.RejectedByUserId,
                dto.RejectedAtUtc,
                dto.Notes);

            var result = await _rejectPayoutHandler.HandleAsync(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(new { error = result.Error });
            }

            var payload = result.Value;

            var response = new RejectPayoutResponseDto
            {
                PayoutId = payload.PayoutId,
                TenantId = payload.TenantId,
                MerchantId = payload.MerchantId,
                Amount = payload.Amount,
                Currency = payload.Currency,
                Status = payload.Status,
                Notes = payload.Notes
            };

            return Ok(response);
        }
    }
}