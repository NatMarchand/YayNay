using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Events;
using NatMarchand.YayNay.Core.Domain.Infrastructure;

namespace NatMarchand.YayNay.Core.Infrastructure.Events
{
    public class ProjectSessionRequested : IEventProcessor<SessionRequested>
    {
        private readonly ILogger<ProjectSessionRequested> _logger;
        private readonly ISessionRepository _sessionRepository;
        private readonly ISessionProjectionStore _sessionProjectionStore;
        private readonly IPersonQueries _personQueries;

        public ProjectSessionRequested(ILogger<ProjectSessionRequested> logger, ISessionRepository sessionRepository, ISessionProjectionStore sessionProjectionStore, IPersonQueries personQueries)
        {
            _logger = logger;
            _sessionRepository = sessionRepository;
            _sessionProjectionStore = sessionProjectionStore;
            _personQueries = personQueries;
        }

        public async Task DispatchAsync(SessionRequested domainEvent)
        {
            _logger.LogInformation($"Session requested {domainEvent.Id}");

            var session = await _sessionRepository.GetAsync(domainEvent.Id);
            if (session != null)
            {
                var speakers = await Task.WhenAll(session.Speakers.Select(s => _personQueries.GetPersonNameAsync(s)));
                var projection = new SessionProjection(session.Id, session.Title, session.Description, session.Schedule, session.Status, session.Tags, speakers);
                await _sessionProjectionStore.MergeProjectionAsync(projection);
            }
        }
    }

    public class SessionProjection
    {
        public SessionId Id { get; }
        public string Title { get; }
        public string Description { get; }
        public Schedule? Schedule { get; }
        public SessionStatus Status { get; }
        public IReadOnlyCollection<string> Tags { get; }
        public IReadOnlyCollection<PersonName> Speakers { get; }

        public SessionProjection(SessionId id, string title, string description, Schedule? schedule, SessionStatus status, IReadOnlyCollection<string> tags, IReadOnlyCollection<PersonName> speakers)
        {
            Id = id;
            Title = title;
            Description = description;
            Schedule = schedule;
            Status = status;
            Tags = tags;
            Speakers = speakers;
        }
    }

    public interface ISessionProjectionStore
    {
        Task MergeProjectionAsync(SessionProjection projection);
    }
}