using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using yoksdotnet.common;

namespace yoksdotnet.logic;

public static class OptionsStorage
{
    public readonly static string OptionsDirName = "yoksdotnet";

    public readonly static string OptionsFileName = "ydn-options.json";
    public readonly static string PaletteDataFileName = "ydn-custom-palettes.json";

    private readonly static string _appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    private readonly static string _optionsDirPath;
    private readonly static string _optionsPath;
    private readonly static string _paletteDataPath;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        IncludeFields = true,
        Converters =
        {
            new JsonStringEnumConverter(),
            new JsonSfeConverterFactory(),
        },
    };

    static OptionsStorage()
    {
        _optionsDirPath = Path.Combine(_appDataPath, OptionsDirName);

        _optionsPath = Path.Combine(_optionsDirPath, OptionsFileName);
        _paletteDataPath = Path.Combine(_optionsDirPath, PaletteDataFileName);
    }

    public static void Save(ScrOptions options)
    {
        Directory.CreateDirectory(_optionsDirPath);

        var serialized = JsonSerializer.Serialize(options, _jsonOptions);
        File.WriteAllText(_optionsPath, serialized);
    }

    public static void SaveCustomPalettes(List<CustomPaletteSet> palettes)
    {
        Directory.CreateDirectory(_optionsDirPath);

        var serialized = JsonSerializer.Serialize(palettes, _jsonOptions);
        File.WriteAllText(_paletteDataPath, serialized);
    }

    public static ScrOptions? Load()
    {
        try
        {
            var optionsJson = File.ReadAllText(_optionsPath);
            var palettesJson = File.ReadAllText(_paletteDataPath);

            var loadedOptions = JsonSerializer.Deserialize<ScrOptions>(optionsJson, _jsonOptions);
            var loadedCustomPalettes = JsonSerializer.Deserialize<List<CustomPaletteSet>>(palettesJson, _jsonOptions);

            if (loadedOptions is null)
            {
                return null;
            }

            if (loadedCustomPalettes is not null)
            {
                loadedOptions.customPalettes = loadedCustomPalettes;
            }

            return loadedOptions;
        }
        catch (Exception ex) when
        (
            ex is FileNotFoundException ||
            ex is DirectoryNotFoundException ||
            ex is JsonException
        )
        {
            return null;
        }
    }
}
