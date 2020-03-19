using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using NatMarchand.YayNay.Core.Domain.Queries.Person;

namespace NatMarchand.YayNay.Core.Domain.Entities
{
    public class Session
    {
        public SessionId Id { get; }
        public IReadOnlyCollection<PersonId> Speakers { get; }
        public string Title { get; }
        public string Description { get; }
        public IReadOnlyCollection<string> Tags { get; }
        public Schedule? Schedule { get; private set; }
        public SessionStatus Status { get; private set; }

        public Session(SessionId id, IEnumerable<PersonId> speakers, string title, string description, IEnumerable<string> tags, Schedule? schedule, SessionStatus status)
        {
            Id = id;
            Speakers = new HashSet<PersonId>(speakers);
            Title = title;
            Description = description;
            Tags = new HashSet<string>(tags, StringComparer.InvariantCultureIgnoreCase);
            Schedule = schedule;
            Status = status;
        }

        public void Approve(PersonProfile approver, string comment)
        {
            switch (Status)
            {
                case SessionStatus.Requested:
                case SessionStatus.Rejected:
                    Status = SessionStatus.Approved;
                    return;
                default:
                    throw new NotSupportedException($"Session is {Status}");
            }
        }

        public void Reject(PersonProfile approver, string comment)
        {
            switch (Status)
            {
                case SessionStatus.Requested:
                case SessionStatus.Approved:
                    Status = SessionStatus.Rejected;
                    return;
                default:
                    throw new NotSupportedException($"Session is {Status}");
            }
        }

        public void SetSchedule(Schedule? schedule)
        {
            if (Status == SessionStatus.Scheduled && schedule == default)
            {
                throw new NotSupportedException("Cannot reschedule without new schedule");
            }
            
            schedule ??= Schedule;
            if (schedule == null)
            {
                throw new NotSupportedException("Session has no schedule");
            }

            Schedule = schedule;
            Status = SessionStatus.Scheduled;
        }
    }

    [ExcludeFromCodeCoverage]
    [TypeConverter(typeof(IdTypeConverter<SessionId>))]
    public class SessionId : Id
    {
        private SessionId(Guid value)
            : base(value)
        {
        }

        public static implicit operator SessionId(Guid id) => new SessionId(id);
        public static SessionId New() => new SessionId(Guid.NewGuid());
    }

    [ExcludeFromCodeCoverage]
    [TypeConverter(typeof(IdTypeConverter<PersonId>))]
    public class PersonId : Id
    {
        private PersonId(Guid value)
            : base(value)
        {
        }

        public static implicit operator PersonId(Guid id) => new PersonId(id);
        public static PersonId New() => new PersonId(Guid.NewGuid());
    }
}
