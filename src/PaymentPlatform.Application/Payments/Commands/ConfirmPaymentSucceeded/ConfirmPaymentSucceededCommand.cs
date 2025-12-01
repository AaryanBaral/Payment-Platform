

using System.Windows.Input;
using PaymentPlatform.Application.Common;
using PaymentPlatform.Application.Messaging;

namespace PaymentPlatform.Application.Commands.ConfirmPaymentSucceeded
{
    public class ConfirmPaymentSucceededCommand : ICommand<Result>

    {
        public Guid PaymentId { get; }
        public DateTimeOffset CompletedAtUtc { get; }

        public ConfirmPaymentSucceededCommand(Guid paymentId, DateTimeOffset completedAtUtc)
        {
            PaymentId = paymentId;
            CompletedAtUtc = completedAtUtc;
        }
    }
}