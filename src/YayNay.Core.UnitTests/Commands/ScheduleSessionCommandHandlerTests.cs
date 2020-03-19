using System;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain;
using NatMarchand.YayNay.Core.Domain.Commands.ScheduleSession;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Events;
using NatMarchand.YayNay.Core.Domain.Queries.Person;
using NatMarchand.YayNay.Tests.Common.Fakes;
using NFluent;
using Xunit;

namespace NatMarchand.YayNay.Core.UnitTests.Commands
{
    public class ScheduleSessionCommandHandlerTests
    {
        [Fact(DisplayName = "Commands/" + nameof(ScheduleSessionCommandHandler) + "/" + nameof(CheckThat_SchedulingApprovedSession_IsSuccessAndSessionIsScheduled))]
        public async Task CheckThat_SchedulingApprovedSession_IsSuccessAndSessionIsScheduled()
        {
            var existingSession = new Session(SessionId.New(), Array.Empty<PersonId>(), "title", "description", Array.Empty<string>(), default, SessionStatus.Approved);
            var sessionStore = new FakeSessionStore();
            sessionStore.AddSession(existingSession);
            var command = new ScheduleSession(existingSession.Id, new PersonProfile(PersonId.New(), "toto", new[] { UserRight.ScheduleSession }), new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 1, 1, 1, 0, 0, TimeSpan.Zero));
            var sut = new ScheduleSessionCommandHandler(sessionStore);
            var (result, events) = await sut.ExecuteAsync(command);
            Check.That(result).InheritsFrom<SuccessCommandResult>();
            Check.That(events).CountIs(1);
            Check.That(events[0]).IsInstanceOf<SessionScheduled>();
            var sessionRequestedEvent = (SessionScheduled) events[0];
            var session = sessionStore.Sessions[sessionRequestedEvent.Id];
            Check.That(session.Status).IsEqualTo(SessionStatus.Scheduled);
        }
        
        [Fact(DisplayName = "Commands/" + nameof(ScheduleSessionCommandHandler) + "/" + nameof(CheckThat_SchedulingApprovedSessionWithSchedule_IsSuccessAndSessionIsScheduled))]
        public async Task CheckThat_SchedulingApprovedSessionWithSchedule_IsSuccessAndSessionIsScheduled()
        {
            var existingSession = new Session(SessionId.New(), Array.Empty<PersonId>(), "title", "description", Array.Empty<string>(), Schedule.Create(new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 1, 1, 1, 0, 0, TimeSpan.Zero)), SessionStatus.Approved);
            var sessionStore = new FakeSessionStore();
            sessionStore.AddSession(existingSession);
            var command = new ScheduleSession(existingSession.Id, new PersonProfile(PersonId.New(), "toto", new[] { UserRight.ScheduleSession }), default,default);
            var sut = new ScheduleSessionCommandHandler(sessionStore);
            var (result, events) = await sut.ExecuteAsync(command);
            Check.That(result).InheritsFrom<SuccessCommandResult>();
            Check.That(events).CountIs(1);
            Check.That(events[0]).IsInstanceOf<SessionScheduled>();
            var sessionRequestedEvent = (SessionScheduled) events[0];
            var session = sessionStore.Sessions[sessionRequestedEvent.Id];
            Check.That(session.Status).IsEqualTo(SessionStatus.Scheduled);
        }

        [Fact(DisplayName = "Commands/" + nameof(ScheduleSessionCommandHandler) + "/" + nameof(CheckThat_SchedulingWithoutScheduleApprovedSessionWithNoSchedule_IsSuccessAndSessionIsScheduled))]
        public async Task CheckThat_SchedulingWithoutScheduleApprovedSessionWithNoSchedule_IsSuccessAndSessionIsScheduled()
        {
            var existingSession = new Session(SessionId.New(), Array.Empty<PersonId>(), "title", "description", Array.Empty<string>(), default, SessionStatus.Approved);
            var sessionStore = new FakeSessionStore();
            sessionStore.AddSession(existingSession);
            var command = new ScheduleSession(existingSession.Id, new PersonProfile(PersonId.New(), "toto", new[] { UserRight.ScheduleSession }), default,default);
            var sut = new ScheduleSessionCommandHandler(sessionStore);
            var (result, events) = await sut.ExecuteAsync(command);
            Check.That(result).InheritsFrom<ValidationFailureCommandResult>();
            Check.That(events).CountIs(0);
        }
        
        // [Fact(DisplayName = "Commands/" + nameof(ApproveSessionCommandHandler) + "/" + nameof(CheckThat_RejectingRequestedSession_IsSuccessAndSessionIsRejected))]
        // public async Task CheckThat_RejectingRequestedSession_IsSuccessAndSessionIsRejected()
        // {
        //     var existingSession = new Session(SessionId.New(), Array.Empty<PersonId>(), "title", "description", Array.Empty<string>(), default, SessionStatus.Requested);
        //     var sessionStore = new FakeSessionStore();
        //     sessionStore.AddSession(existingSession);
        //     var command = new ApproveSession(new PersonProfile(PersonId.New(), "toto", new[] { UserRight.ApproveSession }), existingSession.Id, false, "hi");
        //     var sut = new ApproveSessionCommandHandler(sessionStore);
        //     var (result, events) = await sut.ExecuteAsync(command);
        //     Check.That(result).InheritsFrom<SuccessCommandResult>();
        //     Check.That(events).CountIs(1);
        //     Check.That(events[0]).IsInstanceOf<SessionRejected>();
        //     var sessionRequestedEvent = (SessionRejected) events[0];
        //     var session = sessionStore.Sessions[sessionRequestedEvent.Id];
        //     Check.That(session.Status).IsEqualTo(SessionStatus.Rejected);
        // }
        //
        // [Fact(DisplayName = "Commands/" + nameof(ApproveSessionCommandHandler) + "/" + nameof(CheckThat_ApprovingApprovedSession_IsValidationFailure))]
        // public async Task CheckThat_ApprovingApprovedSession_IsValidationFailure()
        // {
        //     var existingSession = new Session(SessionId.New(), Array.Empty<PersonId>(), "title", "description", Array.Empty<string>(), default, SessionStatus.Approved);
        //     var sessionStore = new FakeSessionStore();
        //     sessionStore.AddSession(existingSession);
        //     var command = new ApproveSession(new PersonProfile(PersonId.New(), "toto", new[] { UserRight.ApproveSession }), existingSession.Id, true, "hi");
        //     var sut = new ApproveSessionCommandHandler(sessionStore);
        //     var (result, events) = await sut.ExecuteAsync(command);
        //     Check.That(result).InheritsFrom<ValidationFailureCommandResult>();
        //     Check.That(events).CountIs(0);
        // }
        //
        // [Fact(DisplayName = "Commands/" + nameof(ApproveSessionCommandHandler) + "/" + nameof(CheckThat_RejectingRejectedSession_IsValidationFailure))]
        // public async Task CheckThat_RejectingRejectedSession_IsValidationFailure()
        // {
        //     var existingSession = new Session(SessionId.New(), Array.Empty<PersonId>(), "title", "description", Array.Empty<string>(), default, SessionStatus.Rejected);
        //     var sessionStore = new FakeSessionStore();
        //     sessionStore.AddSession(existingSession);
        //     var command = new ApproveSession(new PersonProfile(PersonId.New(), "toto", new[] { UserRight.ApproveSession }), existingSession.Id, false, "hi");
        //     var sut = new ApproveSessionCommandHandler(sessionStore);
        //     var (result, events) = await sut.ExecuteAsync(command);
        //     Check.That(result).InheritsFrom<ValidationFailureCommandResult>();
        //     Check.That(events).CountIs(0);
        // }

        [Fact(DisplayName = "Commands/" + nameof(ScheduleSessionCommandHandler) + "/" + nameof(CheckThat_SchedulingNotExistingSession_IsNotFoundFailure))]
        public async Task CheckThat_SchedulingNotExistingSession_IsNotFoundFailure()
        {
            var sessionStore = new FakeSessionStore();
            var command = new ScheduleSession(SessionId.New(), new PersonProfile(PersonId.New(), "toto", new[] { UserRight.ScheduleSession }), default, default);
            var sut = new ScheduleSessionCommandHandler(sessionStore);
            var (result, events) = await sut.ExecuteAsync(command);
            Check.That(result).InheritsFrom<NotFoundCommandResult>();
            Check.That(events).CountIs(0);
        }

        [Fact(DisplayName = "Commands/" + nameof(ScheduleSessionCommandHandler) + "/" + nameof(CheckThat_SchedulingWithNotTheRight_IsDeniedFailure))]
        public async Task CheckThat_SchedulingWithNotTheRight_IsDeniedFailure()
        {
            var sessionStore = new FakeSessionStore();
            var command = new ScheduleSession(SessionId.New(), new PersonProfile(PersonId.New(), "toto", new UserRight[0]), default, default);
            var sut = new ScheduleSessionCommandHandler(sessionStore);
            var (result, events) = await sut.ExecuteAsync(command);
            Check.That(result).InheritsFrom<DeniedCommandResult>();
            Check.That(events).CountIs(0);
        }
    }
}
