using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace NatMarchand.YayNay.Core.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    [TypeConverter(typeof(IdTypeConverter<PersonId>))]
    public class PersonId : Id
    {
        private PersonId(Guid value)
            : base(value)
        {
        }

        public static implicit operator PersonId(Guid id) => new PersonId(id);
        public static PersonId New() => new PersonId(Guid.NewGuid());
    }
}
