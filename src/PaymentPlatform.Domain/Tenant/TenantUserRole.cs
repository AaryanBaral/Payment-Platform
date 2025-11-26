using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentPlatform.Domain.Tenant
{
    public enum TenantUserRole
    {
        Admin = 1,
        Finance = 2,
        Viewer = 3
    }
}