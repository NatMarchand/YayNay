using System;
using NatMarchand.YayNay.Core.Domain.Entities;
using NFluent;
using Xunit;

namespace NatMarchand.YayNay.Core.UnitTests.Entities
{
    public class ScheduleTests
    {
        [Fact(DisplayName = "Entities/" + nameof(Schedule) + "/" + nameof(CheckThat_ScheduleWithStartBeforeEnd_IsValid))]
        public void CheckThat_ScheduleWithStartBeforeEnd_IsValid()
        {
            var start = new DateTimeOffset(2020, 01, 01, 0, 0, 0, TimeSpan.Zero);
            var end = new DateTimeOffset(2020, 01, 01, 1, 0, 0, TimeSpan.Zero);
            var schedule = Schedule.Create(start, end);
            Check.That(schedule).IsNotNull();
            Check.That(schedule!.StartTime).IsEqualTo(start);
            Check.That(schedule!.EndTime).IsEqualTo(end);
            Check.That(schedule!.Duration).IsEqualTo(TimeSpan.FromHours(1));
        }

        [Fact(DisplayName = "Entities/" + nameof(Schedule) + "/" + nameof(CheckThat_ScheduleWithStartEqualsEnd_ThrowsException))]
        public void CheckThat_ScheduleWithStartEqualsEnd_ThrowsException()
        {
            var start = new DateTimeOffset(2020, 01, 01, 1, 0, 0, TimeSpan.Zero);
            Check.ThatCode(() => Schedule.Create(start, start))
                .Throws<ArgumentOutOfRangeException>()
                .WithMessage("End time cannot be before start time (Parameter 'endTime')");
        }

        [Fact(DisplayName = "Entities/" + nameof(Schedule) + "/" + nameof(CheckThat_ScheduleWithStartAfterEnd_ThrowsException))]
        public void CheckThat_ScheduleWithStartAfterEnd_ThrowsException()
        {
            var start = new DateTimeOffset(2020, 01, 01, 1, 0, 0, TimeSpan.Zero);
            var end = new DateTimeOffset(2020, 01, 01, 0, 0, 0, TimeSpan.Zero);
            Check.ThatCode(() => Schedule.Create(start, end))
                .Throws<ArgumentOutOfRangeException>()
                .WithMessage("End time cannot be before start time (Parameter 'endTime')");
        }
    }
}