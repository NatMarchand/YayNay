using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NatMarchand.YayNay.ApiApp;
using NFluent;
using TechTalk.SpecFlow;
using Xunit.Abstractions;

#nullable disable
namespace NatMarchand.YayNay.IntegrationTests
{
    public abstract class Bindings : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpRequestMessage _request;
        private HttpResponseMessage _response;

        protected IServiceProvider Services => _factory.Services;

        protected Bindings(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _factory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureLogging(logging =>
                        {
                            logging.SetMinimumLevel(LogLevel.Debug);
                            logging.AddConsole();
                            logging.AddDebug();
                        })
                        .ConfigureTestServices(ConfigureTestServices);
                });
            _request = new HttpRequestMessage();
        }

        protected virtual void ConfigureTestServices(IServiceCollection services)
        {
        }

        [When("(GET|HEAD|POST|PUT|DELETE) to (.+)")]
        public void WhenCallingVerbToUri(string verb, string uri)
        {
            _request.Method = new HttpMethod(verb);
            _request.RequestUri = new Uri(uri, UriKind.RelativeOrAbsolute);
        }

        [When("content with type (.+)")]
        public void WhenCallingWithContent(string mimeType, string content)
        {
            _request.Content = new StringContent(content, Encoding.UTF8, mimeType);
        }

        [BeforeScenarioBlock]
        public async Task BeforeThen(ScenarioContext context)
        {
            if (context.CurrentScenarioBlock == ScenarioBlock.Then)
            {
                var client = _factory.CreateClient();
                _response = await client.SendAsync(_request);
            }
        }

        [Then("status code is (.+)")]
        public void ThenStatusCodeIs(string expectedStatusCode)
        {
            Check.That(_response.StatusCode).IsEqualTo(Enum.Parse<HttpStatusCode>(expectedStatusCode, true));
        }

        [Then("content is")]
        public async Task ThenContentIs(string expectedContent)
        {
            var actualContent = await _response.Content.ReadAsStringAsync();
            Check.That(actualContent)
                .IsEqualTo(expectedContent);
        }

        [Then("content matches")]
        public async Task ThenContentMatches(string expectedContent)
        {
            var actualContent = await _response.Content.ReadAsStringAsync();
            Check.That(actualContent)
                .MatchesWildcards(expectedContent);
        }

        public void Dispose()
        {
            _request?.Dispose();
            _response?.Dispose();
            _factory?.Dispose();
        }
    }
}