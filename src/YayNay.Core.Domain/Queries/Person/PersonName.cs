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
        private readonly HashSet<UserRight> _rights;
        
        public PersonId Id { get; }
        public string Name { get; }
        public ICollection<UserRight> Rights => _rights;

        public PersonProfile(PersonId id, string name, IEnumerable<UserRight> rights)
        {
            Id = id;
            Name = name;
            _rights = new HashSet<UserRight>(rights);
        }

        public bool HasRight(UserRight right)
        {
            return Rights.Contains(right);
        }
    }

    public enum UserRight
    {
        ApproveSession,
        ScheduleSession
    }
}
