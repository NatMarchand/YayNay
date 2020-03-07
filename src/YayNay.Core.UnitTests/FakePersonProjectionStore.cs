using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Infrastructure;

namespace NatMarchand.YayNay.IntegrationTests
{
    public class FakePersonProjectionStore : IPersonQueries
    {
        private readonly Dictionary<PersonId, PersonName> _personNames;

        public FakePersonProjectionStore()
        {
            _personNames = new Dictionary<PersonId, PersonName>();
        }

        public Task<PersonName?> GetPersonNameAsync(PersonId id)
        {
            return Task.FromResult(_personNames.TryGetValue(id, out var personName) ? personName : default);
        }

        public void AddPerson(PersonId id, string name)
        {
            _personNames.Add(id, new PersonName(id, name));
        }
    }
}