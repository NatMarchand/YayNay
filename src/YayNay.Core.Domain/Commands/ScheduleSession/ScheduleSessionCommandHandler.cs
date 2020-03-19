using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Events;
using NatMarchand.YayNay.Core.Domain.Infrastructure;
using NatMarchand.YayNay.Core.Domain.Queries.Person;

namespace NatMarchand.YayNay.Core.Domain.Commands.ScheduleSession
{
    public class ScheduleSessionCommandHandler : ICommandHandler<ScheduleSession>
    {
        private readonly ISessionRepository _sessionRepository;

        public ScheduleSessionCommandHandler(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<(ICommandResult result, IReadOnlyList<IDomainEvent> events)> ExecuteAsync(ScheduleSession command, CancellationToken cancellationToken = default)
        {
            var events = new List<IDomainEvent>();
            if (!command.Approver.HasRight(UserRight.ScheduleSession))
            {
                return (new DeniedCommandResult($"Right {nameof(UserRight.ScheduleSession)} required"), DomainEvent.None);
            }

            var session = await _sessionRepository.GetAsync(command.Session);
            if (session == null)
            {
                return (new NotFoundCommandResult($"Session {command.Session} was not found"), DomainEvent.None);
            }

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

            try
            {
                session.SetSchedule(schedule);
                events.Add(new SessionScheduled(session.Id, session.Schedule!));
            }
            catch (NotSupportedException e)
            {
                return (new ValidationFailureCommandResult(e.Message), DomainEvent.None);
            }

            await _sessionRepository.UpdateAsync(session);

            return (new SuccessCommandResult(), events);
        }
    }

    public class ScheduleSession : ICommand
    {
        public SessionId Session { get; }
        public PersonProfile Approver { get; }
        public DateTimeOffset? StartTime { get; }
        public DateTimeOffset? EndTime { get; }

        public ScheduleSession(SessionId session, PersonProfile approver, DateTimeOffset? startTime, DateTimeOffset? endTime)
        {
            Session = session;
            Approver = approver;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
