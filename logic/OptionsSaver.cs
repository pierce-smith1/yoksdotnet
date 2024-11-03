using System;
using System.IO;
using System.Text.Json;

namespace yoksdotnet.logic;

public class OptionsSaver
{
    public readonly static string OptionsDirName = "yoksdotnet";
    public readonly static string OptionsFileName = "yokscr.json";

    private readonly string _appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    private readonly string _optionsPath;

    public OptionsSaver()
    {
        _optionsPath = Path.Combine(_appDataPath, OptionsDirName, OptionsFileName);
    }

    public void Save(ScrOptions options)
    {
        var serialized = JsonSerializer.Serialize(options);

        File.WriteAllText(_optionsPath, serialized);
    }

    public ScrOptions? Load()
    {
        var json = File.ReadAllText(_optionsPath);

        var deserialized = JsonSerializer.Deserialize<ScrOptions>(json);
        return deserialized;
    }
}
