using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentPlatform.Api.Models.Payment;
using PaymentPlatform.Application.Common;
using PaymentPlatform.Application.Payments.Commands.CreatePayment;

namespace PaymentPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly CreatePaymentHandler _createPaymentHandler;
        private readonly MarkPaymentSucceededHandler _markPaymentSucceededHandler;

        public PaymentsController(
            CreatePaymentHandler createPaymentHandler,
            MarkPaymentSucceededHandler markPaymentSucceededHandler)
        {
            _createPaymentHandler = createPaymentHandler;
            _markPaymentSucceededHandler = markPaymentSucceededHandler;
        }

        /// <summary>
        /// Tenant tells us: "A payment has started".
        /// We create a Pending payment record.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePaymentAsync(
            [FromBody] CreatePaymentRequestDto dto,
            CancellationToken cancellationToken)
        {
            var command = new CreatePaymentCommand(
                dto.TenantId,
                dto.MerchantId,
                dto.Amount,
                dto.Currency,
                dto.ExternalPaymentId);

            Result<CreatePaymentResult> result =
                await _createPaymentHandler.HandleAsync(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(new { error = result.Error });
            }

            var payload = result.Value;

            var response = new CreatePaymentResponseDto
            {
                PaymentId = payload.PaymentId,
                TenantId = payload.TenantId,
                MerchantId = payload.MerchantId,
                Amount = payload.Amount,
                Currency = payload.Currency,
                Status = "Pending"
            };

            return Ok(response);
        }

        /// <summary>
        /// Called when the gateway confirms the payment succeeded.
        /// This marks the payment as succeeded and writes ledger entries.
        /// </summary>
        [HttpPost("{paymentId:guid}/succeeded")]
        public async Task<IActionResult> MarkPaymentSucceededAsync(
            Guid paymentId,
            [FromBody] MarkPaymentSucceededRequestDto dto,
            CancellationToken cancellationToken)
        {
            var command = new MarkPaymentSucceededCommand(
                dto.TenantId,
                paymentId,
                dto.CompletedAtUtc);

            Result<MarkPaymentSucceededResult> result =
                await _markPaymentSucceededHandler.HandleAsync(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(new { error = result.Error });
            }

            var payload = result.Value;

            var response = new MarkPaymentSucceededResponseDto
            {
                PaymentId = payload.PaymentId,
                TenantId = payload.TenantId,
                MerchantId = payload.MerchantId,
                MerchantAmount = payload.MerchantAmount,
                PlatformAmount = payload.PlatformAmount,
                Currency = payload.Currency
            };

            return Ok(response);
        }
    }
}