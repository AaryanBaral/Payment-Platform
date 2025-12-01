using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentPlatform.Api.Models.Payment
{
    public class CreatePaymentResponseDto
    {
        public Guid PaymentId { get; set; }
        public Guid TenantId { get; set; }
        public Guid MerchantId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = default!;
        public string Status { get; set; } = default!;
    }
}