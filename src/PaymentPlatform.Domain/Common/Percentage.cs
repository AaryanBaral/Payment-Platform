

namespace PaymentPlatform.Domain.Common
{
    public class Percentage(decimal value) : ValueObject
    {
        public decimal Value { get; } = value;

        public static Percentage From(decimal value)
        {
            if (value <= 0 || value >= 100)
            {
                throw new ArgumentException("Percentage must be between 0 and 100 (exclusive).");
            }
            return new Percentage(value);
        }
        public decimal AsFraction() => Value / 100m;

        // Tell the base ValueObject how to compare two Percentage instances.
        // yeild  helps return values one by one rather then returing all of them by once. 
        // returns onl when called keeing the track of its index.
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
        public override string ToString() => $"{Value}%";
    }
}