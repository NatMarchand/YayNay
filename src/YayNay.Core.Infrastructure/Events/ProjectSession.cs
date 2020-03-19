using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Events;
using NatMarchand.YayNay.Core.Domain.Infrastructure;
using NatMarchand.YayNay.Core.Domain.Queries.Person;
using NatMarchand.YayNay.Core.Domain.Queries.Session;

namespace NatMarchand.YayNay.Core.Infrastructure.Events
{
    public class ProjectSession : IEventProcessor<SessionRequested>, IEventProcessor<SessionApproved>, IEventProcessor<SessionRejected>, IEventProcessor<SessionScheduled>
    {
        private readonly ILogger<ProjectSession> _logger;
        private readonly ISessionRepository _sessionRepository;
        private readonly ISessionProjectionStore _sessionProjectionStore;
        private readonly IPersonProjectionStore _personProjectionStore;

        public ProjectSession(ILogger<ProjectSession> logger, ISessionRepository sessionRepository, ISessionProjectionStore sessionProjectionStore, IPersonProjectionStore personProjectionStore)
        {
            _logger = logger;
            _sessionRepository = sessionRepository;
            _sessionProjectionStore = sessionProjectionStore;
            _personProjectionStore = personProjectionStore;
        }

        public async Task DispatchAsync(SessionRequested domainEvent)
        {
            _logger.LogInformation($"Session requested {domainEvent.Id}");
            await ProjectAsync(domainEvent.Id);
        }

        public async Task DispatchAsync(SessionApproved domainEvent)
        {
            _logger.LogInformation($"Session approved {domainEvent.Id}");
            await ProjectAsync(domainEvent.Id);
        }

        public async Task DispatchAsync(SessionRejected domainEvent)
        {
            _logger.LogInformation($"Session rejected {domainEvent.Id}");
            await ProjectAsync(domainEvent.Id);
        }

        public async Task DispatchAsync(SessionScheduled domainEvent)
        {
            _logger.LogInformation($"Session scheduled {domainEvent.Id}");
            await ProjectAsync(domainEvent.Id);
        }

        private async Task ProjectAsync(SessionId id)
        {
            var session = await _sessionRepository.GetAsync(id);
            if (session != null)
            {
                var speakers = await Task.WhenAll(session.Speakers.Select(async s => (await _personProjectionStore.GetNameAsync(s))!));
                var projection = new SessionProjection(session.Id, session.Title, session.Description, session.Schedule, session.Status, session.Tags, speakers);
                await _sessionProjectionStore.MergeProjectionAsync(projection);
            }
        }
    }
}
