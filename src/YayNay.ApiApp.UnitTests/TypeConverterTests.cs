using System;
using System.Text.Json;
using NatMarchand.YayNay.ApiApp.Converters;
using NatMarchand.YayNay.Core.Domain.Entities;
using NFluent;
using Xunit;

namespace NatMarchand.YayNay.ApiApp.UnitTests
{
    public class TypeConverterTests
    {
        [Fact]
        public void CheckThatTypeConverterConverts()
        {
            PersonId id = Guid.Parse("5ccea556-2e9c-47d0-895f-219baba7a2e0");
            var o = new { Id = id, Number = 42, Date = new DateTimeOffset(1234,5,6,7,8,9, TimeSpan.FromHours(10)) };
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new TypeConverterFactory() },
                WriteIndented = true
            };

            var s = JsonSerializer.Serialize(o, options: options);
            Check.That(s)
                .IsEqualTo(@"{
  ""id"": ""5ccea556-2e9c-47d0-895f-219baba7a2e0"",
  ""number"": 42,
  ""date"": ""1234-05-06T07:08:09+10:00""
}");
        }
    }
}