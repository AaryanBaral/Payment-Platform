

using PaymentPlatform.Domain.Merchant;

namespace PaymentPlatform.Application.Presistence
{
    public interface IMerchantRepository
    {
            Task<Merchant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}