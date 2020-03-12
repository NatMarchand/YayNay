using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NatMarchand.YayNay.Core.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public abstract class Id : IEquatable<Id>, IFormattable
    {
        public Guid Value { get; }

        protected Id(Guid value)
        {
            Value = value;
        }

        public virtual bool Equals(Id? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Value.Equals(other.Value);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Id) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(Id? left, Id? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Id? left, Id? right)
        {
            return !Equals(left, right);
        }

        public override string ToString() => Value.ToString();

        public string ToString(string format) => Value.ToString(format);

        public string ToString(string format, IFormatProvider provider) => Value.ToString(format, provider);
    }
    
    [ExcludeFromCodeCoverage]
    public class IdTypeConverter<TId> : TypeConverter where TId : Id
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var method = typeof(TId).GetMethod("op_Implicit", new[] { typeof(Guid) });
            return method.Invoke(null, new object[] { Guid.Parse((string) value) });
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return ((Id) value).ToString();
        }
    }
}