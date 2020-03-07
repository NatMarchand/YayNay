using System;
using System.Collections.Generic;

namespace NatMarchand.YayNay.Core.Domain.Events
{
    public interface IDomainEvent
    {
        
    }

    public static class DomainEvent
    {
        public static readonly IReadOnlyList<IDomainEvent> None = Array.Empty<IDomainEvent>();
    }
}