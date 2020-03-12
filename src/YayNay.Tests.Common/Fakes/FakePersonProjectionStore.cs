using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Queries.Person;

namespace NatMarchand.YayNay.Tests.Common.Fakes
{
    public class FakePersonProjectionStore : IPersonProjectionStore
    {
        private readonly Dictionary<PersonId, PersonName> _personNames;
        private readonly Dictionary<PersonId, PersonProfile> _profiles;

        public FakePersonProjectionStore()
        {
            _personNames = new Dictionary<PersonId, PersonName>();
            _profiles = new Dictionary<PersonId, PersonProfile>();
        }

        public Task<PersonName?> GetNameAsync(PersonId id)
        {
            return Task.FromResult(_personNames.TryGetValue(id, out var personName) ? personName : default);
        }

        public Task<PersonProfile?> GetProfileAsync(string authenticationProvider, string providerId)
        {
            return Task.FromResult(_profiles.TryGetValue(Guid.Parse(providerId), out var profile) ? profile : default);
        }

        public void AddPerson(PersonId id, string name, bool isAdmin = false)
        {
            _personNames.Add(id, new PersonName(id, name));
            _profiles.Add(id, new PersonProfile(id, name, isAdmin ? Enum.GetValues(typeof(UserRight)).Cast<UserRight>() : Array.Empty<UserRight>()));
        }
    }
}