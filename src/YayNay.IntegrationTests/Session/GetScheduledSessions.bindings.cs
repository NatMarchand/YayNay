using TechTalk.SpecFlow;
using Xunit.Abstractions;

namespace NatMarchand.YayNay.IntegrationTests.Session
{
    [Binding, Scope(Feature = "Get scheduled sessions")]
    public class GetScheduledSessionsBindings : SessionBindings
    {
        public GetScheduledSessionsBindings(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }
    }
}
