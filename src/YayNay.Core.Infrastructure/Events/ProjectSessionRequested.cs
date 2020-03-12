using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NatMarchand.YayNay.Core.Domain.Events;
using NatMarchand.YayNay.Core.Domain.Infrastructure;
using NatMarchand.YayNay.Core.Domain.Queries.Person;
using NatMarchand.YayNay.Core.Domain.Queries.Session;

namespace NatMarchand.YayNay.Core.Infrastructure.Events
{
    public class ProjectSessionRequested : IEventProcessor<SessionRequested>
    {
        private readonly ILogger<ProjectSessionRequested> _logger;
        private readonly ISessionRepository _sessionRepository;
        private readonly ISessionProjectionStore _sessionProjectionStore;
        private readonly IPersonProjectionStore _personProjectionStore;

        public ProjectSessionRequested(ILogger<ProjectSessionRequested> logger, ISessionRepository sessionRepository, ISessionProjectionStore sessionProjectionStore, IPersonProjectionStore personProjectionStore)
        {
            _logger = logger;
            _sessionRepository = sessionRepository;
            _sessionProjectionStore = sessionProjectionStore;
            _personProjectionStore = personProjectionStore;
        }

        public async Task DispatchAsync(SessionRequested domainEvent)
        {
            _logger.LogInformation($"Session requested {domainEvent.Id}");

            var session = await _sessionRepository.GetAsync(domainEvent.Id);
            if (session != null)
            {
                var speakers = await Task.WhenAll(session.Speakers.Select(async s => (await _personProjectionStore.GetNameAsync(s))!));
                var projection = new SessionProjection(session.Id, session.Title, session.Description, session.Schedule, session.Status, session.Tags, speakers);
                await _sessionProjectionStore.MergeProjectionAsync(projection);
            }
        }
    }
}