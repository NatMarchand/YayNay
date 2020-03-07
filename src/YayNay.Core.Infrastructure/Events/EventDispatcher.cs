using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NatMarchand.YayNay.Core.Domain.Events;

namespace NatMarchand.YayNay.Core.Infrastructure.Events
{
    public class EventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public EventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync(IReadOnlyList<IDomainEvent> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                var dispatcherType = typeof(IEventProcessor<>).MakeGenericType(domainEvent.GetType());
                var dispatchers = _serviceProvider.GetServices(dispatcherType);

                foreach (var dispatcher in dispatchers)
                {
                    var m = dispatcherType.GetMethod(nameof(IEventProcessor<IDomainEvent>.DispatchAsync));
                    await (Task) m.Invoke(dispatcher, new object[] { domainEvent });
                }
            }
        }
    }
}