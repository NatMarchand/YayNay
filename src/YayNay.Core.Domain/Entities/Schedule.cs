using System;
using System.Diagnostics.CodeAnalysis;

namespace NatMarchand.YayNay.Core.Domain.Entities
{
    public sealed class Schedule : IEquatable<Schedule>
    {
        public DateTimeOffset StartTime { get; }
        public DateTimeOffset EndTime { get; }
        public TimeSpan Duration => EndTime - StartTime;

        private Schedule(DateTimeOffset startTime, DateTimeOffset endTime)
        {
            if (startTime >= endTime)
            {
                throw new ArgumentOutOfRangeException(nameof(endTime), "End time cannot be before start time");
            }

            StartTime = startTime;
            EndTime = endTime;
        }

        public static Schedule? Create(DateTimeOffset? startTime, DateTimeOffset? endTime)
        {
            return (start: startTime, end: endTime) switch
            {
                (null, null) => default,
                (null, _) => throw new ArgumentNullException(nameof(startTime), "Start time cannot be null"),
                (_, null) => throw new ArgumentNullException(nameof(endTime), "End time cannot be null"),
                _ => new Schedule(startTime!.Value, endTime!.Value)
            };
        }

        [ExcludeFromCodeCoverage]
        public bool Equals(Schedule? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return StartTime.Equals(other.StartTime) && EndTime.Equals(other.EndTime);
        }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Schedule) obj);
        }

        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
        {
            return HashCode.Combine(StartTime, EndTime);
        }

        [ExcludeFromCodeCoverage]
        public static bool operator ==(Schedule? left, Schedule? right)
        {
            return Equals(left, right);
        }

        [ExcludeFromCodeCoverage]
        public static bool operator !=(Schedule? left, Schedule? right)
        {
            return !Equals(left, right);
        }
    }
}
