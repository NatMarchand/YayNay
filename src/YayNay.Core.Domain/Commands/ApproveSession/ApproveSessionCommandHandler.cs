using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Events;
using NatMarchand.YayNay.Core.Domain.Infrastructure;
using NatMarchand.YayNay.Core.Domain.Queries.Person;

namespace NatMarchand.YayNay.Core.Domain.Commands.ApproveSession
{
    public class ApproveSessionCommandHandler : ICommandHandler<ApproveSession>
    {
        private readonly ISessionRepository _sessionRepository;

        public ApproveSessionCommandHandler(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<(ICommandResult result, IReadOnlyList<IDomainEvent> events)> ExecuteAsync(ApproveSession command, CancellationToken cancellationToken = default)
        {
            var events = new List<IDomainEvent>();
            if (!command.Approver.HasRight(UserRight.ApproveSession))
            {
                return (new DeniedCommandResult($"Right {nameof(UserRight.ApproveSession)} required"), DomainEvent.None);
            }

            var session = await _sessionRepository.GetAsync(command.Session);
            if (session == null)
            {
                return (new NotFoundCommandResult($"Session {command.Session} was not found"), DomainEvent.None);
            }

            try
            {
                if (command.IsApproved)
                {
                    session.Approve(command.Approver, command.Comment);
                    events.Add(new SessionApproved(session.Id));
                }
                else
                {
                    session.Reject(command.Approver, command.Comment);
                    events.Add(new SessionRejected(session.Id));
                }
            }
            catch (NotSupportedException e)
            {
                return (new ValidationFailureCommandResult(e.Message), DomainEvent.None);
            }

            await _sessionRepository.UpdateAsync(session);
            
            return (new SuccessCommandResult(), events);
        }
    }

    public class ApproveSession : ICommand
    {
        public PersonProfile Approver { get; }
        public SessionId Session { get; }
        public bool IsApproved { get; }
        public string Comment { get; }

        public ApproveSession(PersonProfile approver, SessionId session, bool isApproved, string comment)
        {
            Approver = approver;
            Session = session;
            IsApproved = isApproved;
            Comment = comment;
        }
    }
}