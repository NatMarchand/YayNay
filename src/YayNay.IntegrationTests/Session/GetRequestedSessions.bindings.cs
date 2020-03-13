using System;
using TechTalk.SpecFlow;
using Xunit.Abstractions;

namespace NatMarchand.YayNay.IntegrationTests.Session
{
    [Binding, Scope(Feature = "Get requested sessions")]
    public class GetSessionsByStatusBindings : SessionBindings
    {
        public GetSessionsByStatusBindings(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }
    }
}