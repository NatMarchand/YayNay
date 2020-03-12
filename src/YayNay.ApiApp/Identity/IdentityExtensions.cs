using System;
using System.Security.Claims;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Queries.Person;

namespace NatMarchand.YayNay.ApiApp.Identity
{
    public static class IdentityExtensions
    {
        public static YayNayIdentity? GetIdentity(this ClaimsPrincipal principal)
        {
            return principal.Identity as YayNayIdentity;
        }

        public static PersonProfile? GetProfile(this ClaimsPrincipal principal)
        {
            return principal.GetIdentity()?.Profile;
        }

        public static PersonId GetId(this ClaimsPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated)
            {
                throw new NotSupportedException();
            }

            return principal.GetIdentity()!.Id;
        }
    }
}