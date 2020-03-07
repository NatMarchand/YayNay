using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Events;
using NatMarchand.YayNay.Core.Domain.Infrastructure;

namespace NatMarchand.YayNay.Core.Domain.Commands.RequestSession
{
    public class RequestSessionCommandHandler : ICommandHandler<RequestSession>
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IPersonQueries _personQueries;

        public RequestSessionCommandHandler(ISessionRepository sessionRepository, IPersonQueries personQueries)
        {
            _sessionRepository = sessionRepository;
            _personQueries = personQueries;
        }

        public async Task<(ICommandResult result, IReadOnlyList<IDomainEvent> events)> ExecuteAsync(RequestSession command, CancellationToken cancellationToken = default)
        {
            Schedule? schedule;

            try
            {
                schedule = Schedule.Create(command.StartTime, command.EndTime);
            }
            catch (ArgumentException e)
            {
                var result = new ValidationFailureCommandResult($"Schedule is invalid: {e.Message}.");
                result.AddValidationError(e.ParamName, e.Message);
                return (result, DomainEvent.None);
            }

            if (command.Speakers.Any())
            {
                var peopleExists = await Task.WhenAll(command.Speakers.Select(async s =>
                {
                    var name = await _personQueries.GetPersonNameAsync(s);
                    return (id: s, exists: name != null);
                }));

                ValidationFailureCommandResult? vfcr = null;
                foreach (var p in peopleExists.Where(p => !p.exists))
                {
                    vfcr ??= new ValidationFailureCommandResult("Speakers are invalid: unknown speakers");
                    vfcr.AddValidationError(nameof(RequestSession.Speakers), $"{p.id} is unknown");
                }

                if (vfcr != null)
                {
                    return (vfcr, DomainEvent.None);
                }
            }

            var session = new Session(SessionId.New(), command.Speakers, command.Title, command.Description, command.Tags, schedule, SessionStatus.Requested);

            await _sessionRepository.AddAsync(session).ConfigureAwait(false);

            return (new SuccessCommandResult(), new[] { new SessionRequested(session.Id) });
        }
    }

    public class RequestSession : ICommand
    {
        public IReadOnlyCollection<PersonId> Speakers { get; }
        public string Title { get; }
        public string Description { get; }
        public IEnumerable<string> Tags { get; }
        public DateTimeOffset? StartTime { get; }
        public DateTimeOffset? EndTime { get; }

        public RequestSession(IReadOnlyCollection<PersonId> speakers, string title, string description, IEnumerable<string> tags, DateTimeOffset? startTime, DateTimeOffset? endTime)
        {
            Speakers = speakers;
            Title = title;
            Description = description;
            Tags = tags;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}