using System;
using TechTalk.SpecFlow;
using Xunit.Abstractions;

namespace NatMarchand.YayNay.IntegrationTests.Session
{
    [Binding, Scope(Feature = "Get requested sessions")]
    public class GetRequestedSessionsBindings : SessionBindings
    {
        public GetRequestedSessionsBindings(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }
    }
}
