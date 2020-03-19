using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace NatMarchand.YayNay.Core.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    [TypeConverter(typeof(IdTypeConverter<SessionId>))]
    public class SessionId : Id
    {
        private SessionId(Guid value)
            : base(value)
        {
        }

        public static implicit operator SessionId(Guid id) => new SessionId(id);
        public static SessionId New() => new SessionId(Guid.NewGuid());
    }
}
