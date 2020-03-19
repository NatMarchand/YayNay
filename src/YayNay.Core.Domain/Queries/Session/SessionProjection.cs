using System;
using System.Collections.Generic;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.PlanningContext.Entities;
using NatMarchand.YayNay.Core.Domain.Queries.Person;

namespace NatMarchand.YayNay.Core.Domain.Queries.Session
{
    public class SessionProjection
    {
        public SessionId Id { get; }
        public string Title { get; }
        public string Description { get; }
        public Schedule? Schedule { get; }
        public SessionStatus Status { get; }
        public IReadOnlyCollection<string> Tags { get; }
        public IReadOnlyCollection<PersonName> Speakers { get; }

        public SessionProjection(SessionId id, string title, string description, Schedule? schedule, SessionStatus status, IReadOnlyCollection<string> tags, IReadOnlyCollection<PersonName> speakers)
        {
            Id = id;
            Title = title;
            Description = description;
            Schedule = schedule;
            Status = status;
            Tags = tags;
            Speakers = speakers;
        }
    }
}
