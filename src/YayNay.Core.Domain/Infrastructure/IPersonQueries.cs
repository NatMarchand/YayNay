using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Entities;

namespace NatMarchand.YayNay.Core.Domain.Infrastructure
{
    public interface IPersonQueries
    {
        Task<PersonName?> GetPersonNameAsync(PersonId id);
    }

    public class PersonName
    {
        public PersonId Id { get; }
        public string Name { get; }

        public PersonName(PersonId id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}