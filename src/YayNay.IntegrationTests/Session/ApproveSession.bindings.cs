using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using Xunit.Abstractions;

namespace NatMarchand.YayNay.IntegrationTests.Session
{
    [Binding, Scope(Feature = "Approve session")]
    public class ApproveSessionBindings : SessionCommandBindings
    {
        public ApproveSessionBindings(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }
}