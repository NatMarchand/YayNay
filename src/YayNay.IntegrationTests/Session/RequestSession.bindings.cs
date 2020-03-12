using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Infrastructure;
using NatMarchand.YayNay.Core.Domain.Queries.Session;
using NatMarchand.YayNay.Tests.Common.Fakes;
using NFluent;
using TechTalk.SpecFlow;
using Xunit.Abstractions;

#nullable disable
namespace NatMarchand.YayNay.IntegrationTests.Session
{
    [Binding, Scope(Feature = "Request Session")]
    public class RequestSessionBindings : Bindings
    {
        private Core.Domain.Entities.Session _session;
        private SessionProjection _sessionProjection;

        private FakeSessionRepository SessionRepository => Services.GetRequiredService<FakeSessionRepository>();
        private FakeSessionProjectionStore SessionProjectionStore => Services.GetRequiredService<FakeSessionProjectionStore>();
        private FakePersonProjectionStore PersonProjectionStore => Services.GetRequiredService<FakePersonProjectionStore>();

        public RequestSessionBindings(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }


        protected override void ConfigureTestServices(IServiceCollection services)
        {
            services.AddSingleton<FakeSessionRepository>();
            services.AddTransient<ISessionRepository>(p => p.GetRequiredService<FakeSessionRepository>());

            services.AddSingleton<FakeSessionProjectionStore>();
            services.AddTransient<ISessionProjectionStore>(p => p.GetRequiredService<FakeSessionProjectionStore>());

            base.ConfigureTestServices(services);
        }

        [Given("person (.+) named (.+)")]
        public void GivenPersonNamed(Guid personId, string name)
        {
            PersonProjectionStore.AddPerson(personId, name);
        }

        [Then("session store is empty")]
        public void ThenSessionStoreIsEmpty()
        {
            Check.That(SessionRepository.Sessions).CountIs(0);
            Check.That(SessionProjectionStore.Projections).CountIs(0);
        }

        [Then("session store contains one entitled (.+)")]
        public void ThenSessionStoreContains(string title)
        {
            _session = SessionRepository.Sessions.Values.FirstOrDefault(s => s.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase));
            Check.That(_session).IsNotNull();

            _sessionProjection = SessionProjectionStore.Projections.Values.FirstOrDefault(s => s.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase));
            Check.That(_sessionProjection).IsNotNull();
        }

        [Then("description is")]
        public void ThenDescriptionIs(string description)
        {
            Check.That(_session.Description).IsEqualTo(description);
            Check.That(_sessionProjection.Description).IsEqualTo(description);
        }

        [Then("start time is *(.*)")]
        public void ThenStartTimeIs(DateTimeOffset? startTime)
        {
            Check.That(_session.Schedule?.StartTime).IsEqualTo(startTime);
            Check.That(_sessionProjection.Schedule?.StartTime).IsEqualTo(startTime);
        }

        [Then("end time is *(.*)")]
        public void ThenEndTimeIs(DateTimeOffset? endTime)
        {
            Check.That(_session.Schedule?.EndTime).IsEqualTo(endTime);
            Check.That(_sessionProjection.Schedule?.EndTime).IsEqualTo(endTime);
        }

        [Then("tags are *(.*)")]
        public void ThenTagsAre(string tags)
        {
            Check.That(string.Join(", ", _session.Tags)).IsEqualTo(tags);
            Check.That(string.Join(", ", _sessionProjection.Tags)).IsEqualTo(tags);
        }

        [Then("speakers are *(.*)")]
        public void ThenSpeakerAre(string speakers)
        {
            Check.That(string.Join(", ", _session.Speakers)).IsEqualTo(speakers);
            Check.That(string.Join(", ", _sessionProjection.Speakers.Select(p => p.Id))).IsEqualTo(speakers);
        }

        [Then("status is requested")]
        public void ThenStatusIsRequested()
        {
            Check.That(_session.Status).IsEqualTo(SessionStatus.Requested);
            Check.That(_sessionProjection.Status).IsEqualTo(SessionStatus.Requested);
        }
    }
}