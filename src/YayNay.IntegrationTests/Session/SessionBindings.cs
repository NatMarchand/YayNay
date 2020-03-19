using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Infrastructure;
using NatMarchand.YayNay.Core.Domain.Queries.Person;
using NatMarchand.YayNay.Core.Domain.Queries.Session;
using NatMarchand.YayNay.Tests.Common.Fakes;
using NFluent;
using TechTalk.SpecFlow;
using Xunit.Abstractions;

#nullable disable
namespace NatMarchand.YayNay.IntegrationTests.Session
{
    public abstract class SessionBindings : Bindings
    {
        protected FakeSessionStore SessionStore => Services.GetRequiredService<FakeSessionStore>();

        protected SessionBindings(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        protected override void ConfigureTestServices(IServiceCollection services)
        {
            services.AddSingleton<FakeSessionStore>();
            services.AddTransient<ISessionRepository>(p => p.GetRequiredService<FakeSessionStore>());
            services.AddTransient<ISessionProjectionStore>(p => p.GetRequiredService<FakeSessionStore>());

            base.ConfigureTestServices(services);
        }


        [Given("a session entitled (.+) with status (.+)")]
        public void GivenSession(string title, SessionStatus status)
        {
            var session = new Core.Domain.Entities.Session(SessionId.New(), Array.Empty<PersonId>(), title, string.Empty, Array.Empty<string>(), default, status);
            SessionStore.AddSession(session);
        }

        [Given("a session entitled (.+) with status (.+) and id ([0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12})")]
        public void GivenSession(string title, SessionStatus status, Guid id)
        {
            var session = new Core.Domain.Entities.Session(id, Array.Empty<PersonId>(), title, string.Empty, Array.Empty<string>(), default, status);
            SessionStore.AddSession(session);
        }

        [Given("a session entitled (.+) with status (.+) and id ([0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}) between (.+) and (.+)")]
        public void GivenSession(string title, SessionStatus status, Guid id, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            var session = new Core.Domain.Entities.Session(id, Array.Empty<PersonId>(), title, string.Empty, Array.Empty<string>(), Schedule.Create(startTime, endTime), status);
            SessionStore.AddSession(session);
        }
    }

    public abstract class SessionCommandBindings : SessionBindings
    {
        protected Core.Domain.Entities.Session Session { get; private set; }
        protected SessionProjection SessionProjection { get; private set; }

        protected SessionCommandBindings(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }


        [Then("session store is empty")]
        public void ThenSessionStoreIsEmpty()
        {
            Check.That(SessionStore.Sessions).CountIs(0);
            Check.That(SessionStore.Projections).CountIs(0);
        }

        [Then("session store contains one entitled (.+)")]
        public void ThenSessionStoreContains(string title)
        {
            Session = SessionStore.Sessions.Values.FirstOrDefault(s => s.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase));
            Check.That(Session).IsNotNull();

            SessionProjection = SessionStore.Projections.Values.FirstOrDefault(s => s.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase));
            Check.That(SessionProjection).IsNotNull();
        }

        [Then("description is")]
        public void ThenDescriptionIs(string description)
        {
            Check.That(Session.Description).IsEqualTo(description);
            Check.That(SessionProjection.Description).IsEqualTo(description);
        }

        [Then("start time is *(.*)")]
        public void ThenStartTimeIs(DateTimeOffset? startTime)
        {
            Check.That(Session.Schedule?.StartTime).IsEqualTo(startTime);
            Check.That(SessionProjection.Schedule?.StartTime).IsEqualTo(startTime);
        }

        [Then("end time is *(.*)")]
        public void ThenEndTimeIs(DateTimeOffset? endTime)
        {
            Check.That(Session.Schedule?.EndTime).IsEqualTo(endTime);
            Check.That(SessionProjection.Schedule?.EndTime).IsEqualTo(endTime);
        }

        [Then("tags are *(.*)")]
        public void ThenTagsAre(string tags)
        {
            Check.That(string.Join(", ", Session.Tags)).IsEqualTo(tags);
            Check.That(string.Join(", ", SessionProjection.Tags)).IsEqualTo(tags);
        }

        [Then("speakers are *(.*)")]
        public void ThenSpeakerAre(string speakers)
        {
            Check.That(string.Join(", ", Session.Speakers)).IsEqualTo(speakers);
            Check.That(string.Join(", ", SessionProjection.Speakers.Select(p => p.Id))).IsEqualTo(speakers);
        }

        [Then("status is (.+)")]
        public void ThenStatusIs(SessionStatus status)
        {
            Check.That(Session.Status).IsEqualTo(status);
            Check.That(SessionProjection.Status).IsEqualTo(status);
        }
    }
}
