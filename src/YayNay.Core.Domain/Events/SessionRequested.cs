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
}