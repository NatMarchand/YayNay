using System;
using Microsoft.Extensions.DependencyInjection;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Infrastructure;
using NatMarchand.YayNay.Core.Domain.Queries.Person;
using NatMarchand.YayNay.Core.Domain.Queries.Session;
using NatMarchand.YayNay.Tests.Common.Fakes;
using TechTalk.SpecFlow;
using Xunit.Abstractions;

namespace NatMarchand.YayNay.IntegrationTests.Session
{
    [Binding, Scope(Feature = "Get requested sessions")]
    public class GetSessionsByStatusBindings : Bindings
    {
        private FakeSessionRepository SessionRepository => Services.GetRequiredService<FakeSessionRepository>();
        private FakeSessionProjectionStore SessionProjectionStore => Services.GetRequiredService<FakeSessionProjectionStore>();
        

        public GetSessionsByStatusBindings(ITestOutputHelper testOutputHelper)
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

        [Given("a user")]
        public void GivenAUser()
        {
            PersonProjectionStore.AddPerson(TestAuthenticationHandler.UserPersonId, "John doe", false);
        }

        [Given("an admin")]
        public void GivenAnAdmin()
        {
            PersonProjectionStore.AddPerson(TestAuthenticationHandler.AdminPersonId, "John Doe", true);
        }


        [Given("a session entitled (.+)")]
        public void GivenSession(string title)
        {
            SessionProjectionStore.MergeProjectionAsync(new SessionProjection(SessionId.New(), title, string.Empty, default, SessionStatus.Requested, Array.Empty<string>(), Array.Empty<PersonName>()));
        }
    }
}