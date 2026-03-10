using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using yoksdotnet.common;
using yoksdotnet.data;
using yoksdotnet.drawing;

namespace yoksdotnet.logic;

public static class OptionsStorage
{
    public readonly static string OptionsDirName = "yoksdotnet";

    public readonly static string OptionsFileName = "ydn-options.json";
    public readonly static string PaletteDataFileName = "ydn-custom-palettes.json";

    private readonly static string _appDataPath = 
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

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
            new JsonRgbColorConverter(),
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
        ScrOptions? loadedOptions;
        try
        {
            var optionsJson = File.ReadAllText(_optionsPath);

            loadedOptions = JsonSerializer.Deserialize<ScrOptions>(optionsJson, _jsonOptions);

            if (loadedOptions is null)
            {
                Log.Warning("Options file could not deserialize");
                return null;
            }
        }
        catch (Exception ex) when
        (
            ex is FileNotFoundException ||
            ex is DirectoryNotFoundException ||
            ex is JsonException
        )
        {
            Log.Warning(ex, "Options file couldn't be loaded");
            return null;
        }

        try
        {
            var palettesJson = File.ReadAllText(_paletteDataPath);
            var loadedCustomPalettes = JsonSerializer.Deserialize<List<CustomPaletteSet>>(palettesJson, _jsonOptions);

            if (loadedCustomPalettes is null)
            {
                Log.Warning("Custom palettes file could not deserialize");
                return null;
            }

            if (loadedOptions is not null)
            {
                loadedOptions.customPalettes = loadedCustomPalettes;
            }
        }
        catch (Exception ex) when
        (
            ex is FileNotFoundException ||
            ex is DirectoryNotFoundException ||
            ex is JsonException
        )
        {
            Log.Warning(ex, "Custom palette file couldn't be loaded");
        }

        return loadedOptions;
    }
}
