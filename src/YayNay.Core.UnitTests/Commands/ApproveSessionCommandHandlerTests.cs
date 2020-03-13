using System;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain;
using NatMarchand.YayNay.Core.Domain.Commands.ApproveSession;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Events;
using NatMarchand.YayNay.Core.Domain.Queries.Person;
using NatMarchand.YayNay.Tests.Common.Fakes;
using NFluent;
using Xunit;

namespace NatMarchand.YayNay.Core.UnitTests.Commands
{
    public class ApproveSessionCommandHandlerTests
    {
        [Fact(DisplayName = "Commands/" + nameof(ApproveSessionCommandHandler) + "/" + nameof(CheckThat_ApprovingRequestedSession_IsSuccessAndSessionIsApproved))]
        public async Task CheckThat_ApprovingRequestedSession_IsSuccessAndSessionIsApproved()
        {
            var existingSession = new Session(SessionId.New(), Array.Empty<PersonId>(), "title", "description", Array.Empty<string>(), default, SessionStatus.Requested);
            var sessionStore = new FakeSessionStore();
            sessionStore.AddSession(existingSession);
            var command = new ApproveSession(new PersonProfile(PersonId.New(), "toto", new[] { UserRight.AcceptSession }), existingSession.Id, true, "hi");
            var sut = new ApproveSessionCommandHandler(sessionStore);
            var (result, events) = await sut.ExecuteAsync(command);
            Check.That(result).InheritsFrom<SuccessCommandResult>();
            Check.That(events).CountIs(1);
            Check.That(events[0]).IsInstanceOf<SessionApproved>();
            var sessionRequestedEvent = (SessionApproved) events[0];
            var session = sessionStore.Sessions[sessionRequestedEvent.Id];
            Check.That(session.Status).IsEqualTo(SessionStatus.Approved);
        }
        
        [Fact(DisplayName = "Commands/" + nameof(ApproveSessionCommandHandler) + "/" + nameof(CheckThat_RejectingRequestedSession_IsSuccessAndSessionIsRejected))]
        public async Task CheckThat_RejectingRequestedSession_IsSuccessAndSessionIsRejected()
        {
            var existingSession = new Session(SessionId.New(), Array.Empty<PersonId>(), "title", "description", Array.Empty<string>(), default, SessionStatus.Requested);
            var sessionStore = new FakeSessionStore();
            sessionStore.AddSession(existingSession);
            var command = new ApproveSession(new PersonProfile(PersonId.New(), "toto", new[] { UserRight.AcceptSession }), existingSession.Id, false, "hi");
            var sut = new ApproveSessionCommandHandler(sessionStore);
            var (result, events) = await sut.ExecuteAsync(command);
            Check.That(result).InheritsFrom<SuccessCommandResult>();
            Check.That(events).CountIs(1);
            Check.That(events[0]).IsInstanceOf<SessionRejected>();
            var sessionRequestedEvent = (SessionRejected) events[0];
            var session = sessionStore.Sessions[sessionRequestedEvent.Id];
            Check.That(session.Status).IsEqualTo(SessionStatus.Rejected);
        }
        
        [Fact(DisplayName = "Commands/" + nameof(ApproveSessionCommandHandler) + "/" + nameof(CheckThat_ApprovingApprovedSession_IsValidationFailure))]
        public async Task CheckThat_ApprovingApprovedSession_IsValidationFailure()
        {
            var existingSession = new Session(SessionId.New(), Array.Empty<PersonId>(), "title", "description", Array.Empty<string>(), default, SessionStatus.Approved);
            var sessionStore = new FakeSessionStore();
            sessionStore.AddSession(existingSession);
            var command = new ApproveSession(new PersonProfile(PersonId.New(), "toto", new[] { UserRight.AcceptSession }), existingSession.Id, true, "hi");
            var sut = new ApproveSessionCommandHandler(sessionStore);
            var (result, events) = await sut.ExecuteAsync(command);
            Check.That(result).InheritsFrom<ValidationFailureCommandResult>();
            Check.That(events).CountIs(0);
        }
        
        [Fact(DisplayName = "Commands/" + nameof(ApproveSessionCommandHandler) + "/" + nameof(CheckThat_RejectingRejectedSession_IsValidationFailure))]
        public async Task CheckThat_RejectingRejectedSession_IsValidationFailure()
        {
            var existingSession = new Session(SessionId.New(), Array.Empty<PersonId>(), "title", "description", Array.Empty<string>(), default, SessionStatus.Rejected);
            var sessionStore = new FakeSessionStore();
            sessionStore.AddSession(existingSession);
            var command = new ApproveSession(new PersonProfile(PersonId.New(), "toto", new[] { UserRight.AcceptSession }), existingSession.Id, false, "hi");
            var sut = new ApproveSessionCommandHandler(sessionStore);
            var (result, events) = await sut.ExecuteAsync(command);
            Check.That(result).InheritsFrom<ValidationFailureCommandResult>();
            Check.That(events).CountIs(0);
        }
        
        [Fact(DisplayName = "Commands/" + nameof(ApproveSessionCommandHandler) + "/" + nameof(CheckThat_ApprovingNotExistingSession_IsNotFoundFailure))]
        public async Task CheckThat_ApprovingNotExistingSession_IsNotFoundFailure()
        {
            var sessionStore = new FakeSessionStore();
            var command = new ApproveSession(new PersonProfile(PersonId.New(), "toto", new[] { UserRight.AcceptSession }), SessionId.New(), true, "hi");
            var sut = new ApproveSessionCommandHandler(sessionStore);
            var (result, events) = await sut.ExecuteAsync(command);
            Check.That(result).InheritsFrom<NotFoundCommandResult>();
            Check.That(events).CountIs(0);
        }
        
        [Fact(DisplayName = "Commands/" + nameof(ApproveSessionCommandHandler) + "/" + nameof(CheckThat_ApprovingWithNotTheRight_IsDeniedFailure))]
        public async Task CheckThat_ApprovingWithNotTheRight_IsDeniedFailure()
        {
            var sessionStore = new FakeSessionStore();
            var command = new ApproveSession(new PersonProfile(PersonId.New(), "toto", new UserRight[0]), SessionId.New(), true, "hi");
            var sut = new ApproveSessionCommandHandler(sessionStore);
            var (result, events) = await sut.ExecuteAsync(command);
            Check.That(result).InheritsFrom<DeniedCommandResult>();
            Check.That(events).CountIs(0);
        }
    }
}