using System;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain;
using NatMarchand.YayNay.Core.Domain.Commands.RequestSession;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Events;
using NatMarchand.YayNay.Tests.Common.Fakes;
using NFluent;
using Xunit;

namespace NatMarchand.YayNay.Core.UnitTests.Commands
{
    public class RequestSessionCommandHandlerTests
    {
        [Fact(DisplayName = "Commands/" + nameof(RequestSessionCommandHandler) + "/" + nameof(CheckThat_SessionWithSchedule_IsRequested))]
        public async Task CheckThat_SessionWithSchedule_IsRequested()
        {
            var person = PersonId.New();
            var start = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var end = new DateTimeOffset(2020, 1, 1, 1, 0, 0, TimeSpan.Zero);
            var repository = new FakeSessionRepository();
            var sessionQueries = new FakePersonProjectionStore();
            sessionQueries.AddPerson(person, "John Doe");
            var command = new RequestSession(new[] { person }, "super session", "Ca va etre chouette", new[] { "super", "chouette" }, start, end);
            var sut = new RequestSessionCommandHandler(repository, sessionQueries);
            var (result, events) = await sut.ExecuteAsync(command);
            Check.That(result).InheritsFrom<SuccessCommandResult>();
            Check.That(events).CountIs(1);
            Check.That(events[0]).IsInstanceOf<SessionRequested>();
            var sessionRequestedEvent = (SessionRequested) events[0];
            var session = repository.Sessions[sessionRequestedEvent.Id];
            Check.That(session.Speakers).IsEquivalentTo(command.Speakers);
            Check.That(session.Title).IsEqualTo(command.Title);
            Check.That(session.Description).IsEqualTo(command.Description);
            Check.That(session.Tags).IsEquivalentTo(command.Tags);
            Check.That(session.Schedule).IsNotNull().And.IsEqualTo(Schedule.Create(command.StartTime!.Value, command.EndTime!.Value));
            Check.That(session.Status).IsEqualTo(SessionStatus.Requested);
        }

        [Fact(DisplayName = "Commands/" + nameof(RequestSessionCommandHandler) + "/" + nameof(CheckThat_SessionWithNoSchedule_IsRequested))]
        public async Task CheckThat_SessionWithNoSchedule_IsRequested()
        {
            var person = PersonId.New();
            var start = default(DateTimeOffset?);
            var end = default(DateTimeOffset?);
            var repository = new FakeSessionRepository();
            var sessionQueries = new FakePersonProjectionStore();
            sessionQueries.AddPerson(person, "John Doe");
            var command = new RequestSession(new[] { person }, "super session", "Ca va etre chouette", new[] { "super", "chouette" }, start, end);
            var sut = new RequestSessionCommandHandler(repository, sessionQueries);
            var (result, events) = await sut.ExecuteAsync(command);
            Check.That(result).InheritsFrom<SuccessCommandResult>();
            Check.That(events).CountIs(1);
            Check.That(events[0]).IsInstanceOf<SessionRequested>();
            var sessionRequestedEvent = (SessionRequested) events[0];
            var session = repository.Sessions[sessionRequestedEvent.Id];
            Check.That(session.Speakers).IsEquivalentTo(command.Speakers);
            Check.That(session.Title).IsEqualTo(command.Title);
            Check.That(session.Description).IsEqualTo(command.Description);
            Check.That(session.Tags).IsEquivalentTo(command.Tags);
            Check.That(session.Schedule).IsNull();
            Check.That(session.Status).IsEqualTo(SessionStatus.Requested);
        }
        
        [Fact(DisplayName = "Commands/" + nameof(RequestSessionCommandHandler) + "/" + nameof(CheckThat_SessionWithUnknownSpeaker_IsFailed))]
        public async Task CheckThat_SessionWithUnknownSpeaker_IsFailed()
        {
            var person = PersonId.New();
            var start = default(DateTimeOffset?);
            var end = default(DateTimeOffset?);
            var repository = new FakeSessionRepository();
            var sessionQueries = new FakePersonProjectionStore();
            var command = new RequestSession(new[] { person }, "super session", "Ca va etre chouette", new[] { "super", "chouette" }, start, end);
            var sut = new RequestSessionCommandHandler(repository, sessionQueries);
            var (result, events) = await sut.ExecuteAsync(command);
            Check.That(result).InheritsFrom<ValidationFailureCommandResult>();
            Check.That(((ValidationFailureCommandResult) result).Reason).IsEqualTo($"Speakers are invalid: unknown speakers");
            Check.That(((ValidationFailureCommandResult) result).Reason).IsEqualTo($"Speakers are invalid: unknown speakers");
            Check.That(events).CountIs(0);
            Check.That(repository.Sessions).CountIs(0);
        }

        [Fact(DisplayName = "Commands/" + nameof(RequestSessionCommandHandler) + "/" + nameof(CheckThat_SessionWithNoEnd_IsFailed))]
        public async Task CheckThat_SessionWithNoEnd_IsFailed()
        {
            var start = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var end = default(DateTimeOffset?);
            var repository = new FakeSessionRepository();
            var sessionQueries = new FakePersonProjectionStore();
            var command = new RequestSession(new[] { PersonId.New() }, "super session", "Ca va etre chouette", new[] { "super", "chouette" }, start, end);
            var sut = new RequestSessionCommandHandler(repository, sessionQueries);
            var (result, events) = await sut.ExecuteAsync(command);
            Check.That(result).InheritsFrom<ValidationFailureCommandResult>();
            Check.That(((FailureCommandResult) result).Reason).IsEqualTo("Schedule is invalid: End time cannot be null (Parameter 'endTime').");
            Check.That(events).CountIs(0);
            Check.That(repository.Sessions).CountIs(0);
        }

        [Fact(DisplayName = "Commands/" + nameof(RequestSessionCommandHandler) + "/" + nameof(CheckThat_SessionWithNoStart_IsFailed))]
        public async Task CheckThat_SessionWithNoStart_IsFailed()
        {
            var start = default(DateTimeOffset?);
            var end = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var repository = new FakeSessionRepository();
            var sessionQueries = new FakePersonProjectionStore();
            var command = new RequestSession(new[] { PersonId.New() }, "super session", "Ca va etre chouette", new[] { "super", "chouette" }, start, end);
            var sut = new RequestSessionCommandHandler(repository, sessionQueries);
            var (result, events) = await sut.ExecuteAsync(command);
            Check.That(result).IsInstanceOf<ValidationFailureCommandResult>();
            Check.That(((FailureCommandResult) result).Reason).IsEqualTo("Schedule is invalid: Start time cannot be null (Parameter 'startTime').");
            Check.That(events).CountIs(0);
            Check.That(repository.Sessions).CountIs(0);
        }
    }
}