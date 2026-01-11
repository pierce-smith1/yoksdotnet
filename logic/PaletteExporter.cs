using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace yoksdotnet.logic;

public class PaletteExporter
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = false,
    };

    public string Export(List<CustomPaletteEntry> palette)
    {
        var serialized = JsonSerializer.Serialize(palette, _serializerOptions);
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(serialized));
        return encoded;
    }

    public List<CustomPaletteEntry>? Import(string paletteString)
    {
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(paletteString));
        try
        {
            var deserialized = JsonSerializer.Deserialize<List<CustomPaletteEntry>>(decoded, _serializerOptions);
            return deserialized;
        }
        catch
        {
            return null;
        }
    }
}
