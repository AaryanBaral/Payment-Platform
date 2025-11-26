using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentPlatform.Application.Payouts.Commands.GeneratePayoutRequest
{
    public class GeneratePayoutRequestResult
    {
    public Guid PayoutId { get; }
    public Guid TenantId { get; }
    public Guid MerchantId { get; }
    public decimal Amount { get; }
    public string Currency { get; }
    public string Status { get; }

    public GeneratePayoutRequestResult(
        Guid payoutId,
        Guid tenantId,
        Guid merchantId,
        decimal amount,
        string currency,
        string status)
    {
        PayoutId = payoutId;
        TenantId = tenantId;
        MerchantId = merchantId;
        Amount = amount;
        Currency = currency;
        Status = status;
    }
    }
}