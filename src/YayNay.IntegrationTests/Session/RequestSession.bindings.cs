using System;
using TechTalk.SpecFlow;
using Xunit.Abstractions;

#nullable disable
namespace NatMarchand.YayNay.IntegrationTests.Session
{
    [Binding, Scope(Feature = "Request Session")]
    public class RequestSessionBindings : SessionCommandBindings
    {
        public RequestSessionBindings(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Given("person (.+) named (.+)")]
        public void GivenPersonNamed(Guid personId, string name)
        {
            PersonProjectionStore.AddPerson(personId, name);
        }
    }
}