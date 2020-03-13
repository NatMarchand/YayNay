using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NatMarchand.YayNay.ApiApp;
using NatMarchand.YayNay.Core.Domain.Queries.Person;
using NatMarchand.YayNay.Tests.Common;
using NatMarchand.YayNay.Tests.Common.Fakes;
using NFluent;
using TechTalk.SpecFlow;
using Xunit.Abstractions;

#nullable disable
namespace NatMarchand.YayNay.IntegrationTests
{
    public abstract class Bindings : IDisposable
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpRequestMessage _request;
        private HttpResponseMessage _response;

        protected IServiceProvider Services => _factory.Services;
        protected FakePersonProjectionStore PersonProjectionStore => Services.GetRequiredService<FakePersonProjectionStore>();

        protected Bindings(ITestOutputHelper testOutputHelper)
        {
            _factory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureLogging(logging =>
                        {
                            logging.AddFilter("Microsoft", LogLevel.Warning);
                            logging.SetMinimumLevel(LogLevel.Debug);

                            logging.AddProvider(new XUnitLoggerProvider(testOutputHelper));
                        })
                        .ConfigureServices(s =>
                        {
                            s.Configure<ConsoleLifetimeOptions>(o => o.SuppressStatusMessages = true);
                        })
                        .ConfigureTestServices(ConfigureTestServices)
                        .ConfigureTestServices(s =>
                        {
                            s.Configure<JsonOptions>(o => o.JsonSerializerOptions.WriteIndented = true);
                        });
                });
            _request = new HttpRequestMessage();
        }

        protected virtual void ConfigureTestServices(IServiceCollection services)
        {
            var personProjectionStore = new FakePersonProjectionStore();
            personProjectionStore.AddPerson(TestAuthenticationHandler.UserPersonId, "John TheUser", false);
            personProjectionStore.AddPerson(TestAuthenticationHandler.AdminPersonId, "Bob TheAdmin", true);
            services.AddSingleton(personProjectionStore);
            services.AddTransient<IPersonProjectionStore>(p => p.GetRequiredService<FakePersonProjectionStore>());

            services.AddAuthentication(TestAuthenticationHandler.TestScheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(TestAuthenticationHandler.TestScheme, o => { });
        }

        [When("(GET|HEAD|POST|PUT|DELETE) to (.+)")]
        public void WhenCallingVerbToUri(string verb, string uri)
        {
            _request.Method = new HttpMethod(verb);
            _request.RequestUri = new Uri(uri, UriKind.RelativeOrAbsolute);
        }

        [When("header ([^ ]+) is (.*)")]
        public void WhenHeader(string header, string value)
        {
            _request.Headers.TryAddWithoutValidation(header, value);
        }

        [When("authenticated as an admin")]
        public void WhenAuthenticatedAsAnAdmin()
        {
            _request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {TestAuthenticationHandler.AdminToken}");
        }

        [When("authenticated as a user")]
        public void WhenAuthenticatedAsAUser()
        {
            _request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {TestAuthenticationHandler.UserToken}");
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
        public void ThenStatusCodeIs(HttpStatusCode expectedStatusCode)
        {
            Check.That(_response.StatusCode).IsEqualTo(expectedStatusCode);
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