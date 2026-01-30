using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace yoksdotnet.common;

// A "static field enumeration" (SFE) is what I'm calling a pattern where you want
// to define a fixed number of instances for a class, and do so by including
// the instances as public static fields of the class itself.
//
// For example:
//
// class Color
// {
//    public static Color Red = new("#ff0000");
//    public static Color Blue = new("#0000ff");
//
//    public string Hex { get; set; }
//
//    private Color(string hex) { Hex = hex; }
// }
//
// We use this pattern for many of the most important classes in the codebase -
// it's perfect for things that are at heart "enumerations" but also really
// want to have data and behavior attached to them.
//
// The rest of this file is helpers to support the use of this pattern.

// If you plan to use a class as an SFE, you should tag it with this interface.
// This will allow it to be serialized into JSON and used with the below helper functions.
public interface ISfEnum
{
    public string Name { get; }
}

public static class SfEnums
{
    public static IEnumerable<T> GetAll<T>() where T : class, ISfEnum 
    {
        var instances = typeof(T)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => f.GetValue(null))
            .Where(g => g is T && g is not null)
            .Select(g => (g as T)!);

        return instances;
    }
}

// Serializing SFEs is tricky because you don't want to serialize the actual "guts"
// of the object. Just knowing the name and the type is enough to look it up and
// retrieve the actual instance from the class. (You can't construct SFE instances anyway.)
// This converter does exactly that: instances of SFEs are serialized and deserialized by
// just their names.
public class JsonSfeConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        var canConvert = typeToConvert.GetInterfaces().Any(i => i == typeof(ISfEnum));
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

    private class JsonSfeConverter<T>() : JsonConverter<T> where T : class, ISfEnum
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var name = reader.GetString()!;
            if (name is null)
            {
                return null;
            }

            var value = SfEnums.GetAll<T>().FirstOrDefault(v => v.Name == name);
            return value;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Name);
        }
    }
}
