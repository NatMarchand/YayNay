using NatMarchand.YayNay.Core.Domain.Entities;

namespace NatMarchand.YayNay.Core.Domain.Events
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
}