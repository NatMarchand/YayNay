using System;
using System.Collections.Generic;
using NatMarchand.YayNay.Core.Domain.Entities;

namespace NatMarchand.YayNay.Core.Domain.Queries.Person
{
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

    public class PersonProfile
    {
        public PersonId Id { get; }
        public string Name { get; }
        public ISet<UserRight> Rights { get; }

        public PersonProfile(PersonId id, string name, IEnumerable<UserRight> rights)
        {
            Id = id;
            Name = name;
            Rights=new HashSet<UserRight>(rights);
        }

        public bool HasRight(UserRight right)
        {
            return Rights.Contains(right);
        }
    }

    public enum UserRight
    {
        AcceptSession
    }
}