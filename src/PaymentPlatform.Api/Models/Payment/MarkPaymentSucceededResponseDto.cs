using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentPlatform.Api.Models.Payment
{
    public class MarkPaymentSucceededResponseDto
    {
        public Guid PaymentId { get; set; }
        public Guid TenantId { get; set; }
        public Guid MerchantId { get; set; }
        public decimal MerchantAmount { get; set; }
        public decimal PlatformAmount { get; set; }
        public string Currency { get; set; } = default!;
    }
}