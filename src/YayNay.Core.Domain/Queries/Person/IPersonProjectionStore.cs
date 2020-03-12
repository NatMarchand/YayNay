using System;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Entities;

namespace NatMarchand.YayNay.Core.Domain.Queries.Person
{
    public interface IPersonProjectionStore
    {
        Task<PersonName?> GetNameAsync(PersonId id);
        Task<PersonProfile?> GetProfileAsync(string authenticationProvider, string providerId);
    }
}