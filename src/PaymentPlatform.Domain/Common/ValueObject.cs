using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentPlatform.Domain.Common
{
    public abstract class ValueObject
    {
        //  the GetEqualityComponents is called by the child classes and the list of objects are defined by the yield 
        // yield keywords help to return values only when they are needed rather than returing all values at once se done bt return keyword. 
        protected abstract IEnumerable<object?> GetEqualityComponents();


        public override bool Equals(object? obj)
        {
            //it checks if obj is ValueObject
            // if yes it case obj as value object in other.
            if(obj is not ValueObject other)
                return false;

            if(GetType() != other.GetType())
                return false;

            using var thisValues = GetEqualityComponents().GetEnumerator();
            using var otherValues = other.GetEqualityComponents().GetEnumerator();
            while (thisValues.MoveNext() && otherValues.MoveNext())
            {
                var thisCurrent = thisValues.Current;
                var otherCurrent = otherValues.Current;
                if(otherCurrent is null ^ thisCurrent is null)
                    return false;
                
                if (thisCurrent is not null && !thisCurrent.Equals(otherCurrent))
                    return false;
            }
             return !thisValues.MoveNext() && !otherValues.MoveNext();
        }
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Where(x => x is not null)
            .Aggregate(0, (hash, value) => HashCode.Combine(hash, value));
    }
    }
}