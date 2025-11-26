namespace PaymentPlatform.Domain.Common
{
    public class Money : ValueObject
    {
        public string Currency { get; protected set; } = default!;
        public decimal Amount { get; protected set; }
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency.ToUpperInvariant();
        }
        private Money()
        {
            
        }
        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }
        public static Money From(decimal amount, string currency)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount cannot be negative.", nameof(amount));
            }

            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException("Currency is required.", nameof(currency));
            }

            return new Money(amount, currency.Trim());
        }
        public static Money Zero(string currency) => From(0m, currency);
        
        public Money Add(Money other)
        {
            EnsureSameCurrency(other);
            return new Money(Amount + other.Amount, Currency);
        }
        public Money Subtract(Money other)
        {
            EnsureSameCurrency(other);
            var result = Amount - other.Amount;

            if (result < 0)
            {
                throw new InvalidOperationException("Resulting money cannot be negative.");
            }

            return new Money(result, Currency);
        }

        // Multiply money by a scalar (e.g., percentage fraction).
        public Money Multiply(decimal factor)
        {
            if (factor < 0)
            {
                throw new ArgumentException("Factor cannot be negative.", nameof(factor));
            }

            var result = Amount * factor;
            return new Money(result, Currency);
        }

        private void EnsureSameCurrency(Money other)
        {
            if (!Currency.Equals(other.Currency, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Cannot operate on money with different currencies.");
            }
        }


        public override string ToString() => $"{Amount} {Currency}";


    }
}