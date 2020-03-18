using System;
using TechTalk.SpecFlow;
using Xunit.Abstractions;

namespace NatMarchand.YayNay.IntegrationTests.Session
{
    [Binding, Scope(Feature = "Get approved sessions")]
    public class GetApprovedSessionsBindings : SessionBindings
    {
        public GetApprovedSessionsBindings(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }
    }
}
