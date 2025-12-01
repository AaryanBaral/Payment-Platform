using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentPlatform.Api.Models.Payment
{
    public class CreatePaymentRequestDto
    {
        public Guid TenantId { get; set; }
        public Guid MerchantId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = default!;
        public string ExternalPaymentId { get; set; } = default!;
    }
}