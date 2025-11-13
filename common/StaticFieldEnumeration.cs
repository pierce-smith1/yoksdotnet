using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace yoksdotnet.common;

interface IStaticFieldEnumeration
{
    public string Name { get; init; }
}

class StaticFieldEnumeration
{
    public static IEnumerable<T> GetAll<T>() where T : class, IStaticFieldEnumeration 
    {
        var instances = typeof(T)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => f.GetValue(null))
            .Where(g => g is T && g is not null)
            .Select(g => (g as T)!);

        return instances;
    }
}

public class JsonSfeConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        var canConvert = typeToConvert.GetInterfaces().Any(i => i == typeof(IStaticFieldEnumeration));
        return canConvert;
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converter = Activator.CreateInstance(typeof(JsonSfeConverter<>).MakeGenericType(typeToConvert));
        if (converter is JsonConverter jsonConverter)
        {
            return jsonConverter;
        }

        return null;
    }

    private class JsonSfeConverter<T>() : JsonConverter<T> where T : class, IStaticFieldEnumeration
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var name = reader.GetString()!;
            if (name is null)
            {
                return null;
            }

            var value = StaticFieldEnumeration.GetAll<T>().FirstOrDefault(v => v.Name == name);
            return value;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Name);
        }
    }
}
