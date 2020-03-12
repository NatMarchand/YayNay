using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NatMarchand.YayNay.ApiApp.Converters
{
    public class TypeConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.IsPrimitive
                || typeToConvert.IsAssignableFrom(typeof(DateTime))
                || typeToConvert.IsAssignableFrom(typeof(DateTimeOffset)))
            {
                return false;
            }

            var converter = TypeDescriptor.GetConverter(typeToConvert);
            return converter != null && converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string));
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(TypeConverterImpl<>).MakeGenericType(typeToConvert);
            return Activator.CreateInstance(converterType) as JsonConverter;
        }

        private class TypeConverterImpl<T> : JsonConverter<T>
        {
            private readonly TypeConverter _converter;

            public TypeConverterImpl()
            {
                _converter = TypeDescriptor.GetConverter(typeof(T));
            }

            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return (T) _converter.ConvertFromString(reader.GetString());
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(_converter.ConvertToString(value));
            }
        }
    }
}