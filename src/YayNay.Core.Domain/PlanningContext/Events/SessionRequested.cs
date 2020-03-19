using System;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Events;

namespace NatMarchand.YayNay.Core.Domain.PlanningContext.Events
{
    public class SessionRequested : IDomainEvent
    {
        public SessionId Id { get; }

        public SessionRequested(SessionId id)
        {
            Id = id;
        }
    }
    
    public class SessionApproved : IDomainEvent
    {
        public SessionId Id { get; }

        public SessionApproved(SessionId id)
        {
            Id = id;
        }
    }
    
    public class SessionRejected : IDomainEvent
    {
        public SessionId Id { get; }

        public SessionRejected(SessionId id)
        {
            Id = id;
        }
    }
    
    public class SessionScheduled : IDomainEvent
    {
        public SessionId Id { get; }

        public SessionScheduled(SessionId id)
        {
            Id = id;
        }
    }
}
