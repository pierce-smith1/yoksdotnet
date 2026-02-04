using System;
using System.Collections.Generic;
using System.IO;
using yoksdotnet.drawing;

namespace yoksdotnet.logic;

public static class PaletteExporting
{
    public static string Export(CustomPaletteSet paletteGroup)
    {
        // This may look like dramatic overengineering and it sort of is,
        // however the more elegant solution of "just serialize it to JSON"
        // produces awkwardly large codes very quickly.
        // We can afford to be more efficient, so heck, let's just make a
        // binary format.

        using var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        writer.Write((byte)1); // Version, in case this format ever changes
        writer.Write(paletteGroup.Name);
        writer.Write(paletteGroup.Entries.Count);

        foreach (var palette in paletteGroup.Entries)
        {
            writer.Write(palette.Name);

            WriteColor(writer, palette.Palette.scales);
            WriteColor(writer, palette.Palette.scalesShadow);
            WriteColor(writer, palette.Palette.scalesHighlight);
            WriteColor(writer, palette.Palette.horns);
            WriteColor(writer, palette.Palette.hornsShadow);
            WriteColor(writer, palette.Palette.whites);
            WriteColor(writer, palette.Palette.eyes);
        }

        var encoded = Convert.ToBase64String(stream.ToArray());
        return encoded;
    }

    private static void WriteColor(BinaryWriter writer, RgbColor color)
    {
        writer.Write(color.R);
        writer.Write(color.G);
        writer.Write(color.B);
    }

    public static CustomPaletteSet? Import(string encoded)
    {
        try
        {
            var stream = new MemoryStream(Convert.FromBase64String(encoded));
            var reader = new BinaryReader(stream);

            var _version = reader.ReadByte();
            var setName = reader.ReadString();
            var entryCount = reader.ReadInt32();

            List<CustomPaletteEntry> entries = [];

            for (int i = 0; i < entryCount; i++)
            {
                var entryName = reader.ReadString();

                var palette = new Palette
                {
                    scales = ReadColor(reader), 
                    scalesShadow = ReadColor(reader),
                    scalesHighlight = ReadColor(reader), 
                    horns = ReadColor(reader), 
                    hornsShadow = ReadColor(reader), 
                    whites = ReadColor(reader), 
                    eyes = ReadColor(reader)
                };

                var entry = new CustomPaletteEntry(entryName, palette);
                entries.Add(entry);
            }

            var set = new CustomPaletteSet(Guid.NewGuid().ToString(), setName, entries);
            return set;
        }
        catch (Exception ex) when (ex is FormatException || ex is EndOfStreamException)
        {
            return null;
        }
    }

    private static RgbColor ReadColor(BinaryReader reader)
    {
        var r = reader.ReadByte();
        var g = reader.ReadByte();
        var b = reader.ReadByte();

        return new(r, g, b);
    }
}

