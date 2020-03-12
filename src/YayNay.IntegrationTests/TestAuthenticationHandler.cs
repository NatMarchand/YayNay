using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NatMarchand.YayNay.ApiApp.Identity;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Queries.Person;

namespace NatMarchand.YayNay.IntegrationTests
{
    [ExcludeFromCodeCoverage]
    public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IPersonProjectionStore _personProjectionStore;
        public static readonly string TestScheme = "test";
        public static readonly string AdminToken = "adminToken";
        public static readonly string UserToken = "userToken";
        public static readonly PersonId AdminPersonId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public static readonly PersonId UserPersonId = Guid.Parse("00000000-0000-0000-0000-000000000002");

        public TestAuthenticationHandler(IPersonProjectionStore personProjectionStore, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _personProjectionStore = personProjectionStore;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorizationHeader = Request.Headers.TryGetValue("Authorization", out var v) ? v : default;
            Match m;

            if (authorizationHeader == StringValues.Empty || !(m = Regex.Match(authorizationHeader[0], "Bearer (?<token>.+)")).Success)
            {
                return AuthenticateResult.NoResult();
            }

            var token = m.Groups["token"].Value;

            PersonProfile? profile = null;
            if (token.Equals(AdminToken, StringComparison.InvariantCultureIgnoreCase))
            {
                profile = await _personProjectionStore.GetProfileAsync(TestScheme, AdminPersonId.ToString());
            }
            else if (token.Equals(UserToken, StringComparison.InvariantCultureIgnoreCase))
            {
                profile = await _personProjectionStore.GetProfileAsync(TestScheme, UserPersonId.ToString());
            }

            if (profile == null)
            {
                return AuthenticateResult.NoResult();
            }

            var principal = new ClaimsPrincipal(YayNayIdentity.Create(profile, TestScheme));
            var ticket = new AuthenticationTicket(principal, TestScheme);
            return AuthenticateResult.Success(ticket);
        }
    }
}