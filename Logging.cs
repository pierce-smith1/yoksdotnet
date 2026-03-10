using Serilog;
using System;
using System.IO;
using yoksdotnet.logic;

namespace yoksdotnet;

public static class Logging
{
    private readonly static string _appDataPath = 
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    public readonly static string LogFileName = "ydn-log.txt";

    public static void Setup()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(Path.Combine(_appDataPath, OptionsStorage.OptionsDirName, LogFileName), rollOnFileSizeLimit: true)
            .CreateLogger();
    }
}
