using System;
using System.Linq;
using System.Security.Claims;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Queries.Person;

namespace NatMarchand.YayNay.ApiApp.Identity
{
    public class YayNayIdentity : ClaimsIdentity
    {
        public PersonId Id => Profile.Id;
        public PersonProfile Profile { get; }

        public YayNayIdentity(PersonProfile profile, string? authenticationType)
            : base(authenticationType)
        {
            Profile = profile;
        }

        public static YayNayIdentity Create(PersonProfile profile, string authenticationType)
        {
            var identity = new YayNayIdentity(profile, authenticationType);
            identity.AddClaim(new Claim(identity.NameClaimType, profile.Name));
            identity.AddClaims(profile.Rights.Select(r => new Claim(identity.RoleClaimType, r.ToString())));
            return identity;
        }
    }
}