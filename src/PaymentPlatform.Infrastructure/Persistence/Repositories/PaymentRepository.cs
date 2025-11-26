using Microsoft.EntityFrameworkCore;
using PaymentPlatform.Application.Presistence;
using PaymentPlatform.Domain.Payment;

namespace PaymentPlatform.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _dbContext;
        public PaymentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            await _dbContext.Payments.AddAsync(payment, cancellationToken);
        }
        public async Task<Payment?> GetByIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Payments.FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);
        }
    }
}