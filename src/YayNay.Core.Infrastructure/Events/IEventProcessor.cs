using System;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Events;

namespace NatMarchand.YayNay.Core.Infrastructure.Events
{
    public interface IEventProcessor<in TEvent> where TEvent : IDomainEvent
    {
        Task DispatchAsync(TEvent domainEvent);
    }
}