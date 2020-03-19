using System;
using TechTalk.SpecFlow;
using Xunit.Abstractions;

namespace NatMarchand.YayNay.IntegrationTests.Session
{
    [Binding, Scope(Feature = "Schedule Session")]
    public class ScheduleSessionBindings : SessionCommandBindings
    {
        public ScheduleSessionBindings(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }
    }
}
