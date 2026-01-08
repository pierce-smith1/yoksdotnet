using System;
using System.Linq;
using System.Windows;

using yoksdotnet.logic;
using yoksdotnet.windows;

namespace yoksdotnet;

internal record RunType()
{
    internal record Configure() : RunType();
    internal record Show() : RunType();
    internal record Preview(nint WindowHandle) : RunType();
    internal record Debug(bool WithOptions) : RunType();
    internal record DebugPaletteCustomizer() : RunType();
}

public partial class App : Application
{
    public App() {}

    private OptionsWindow? _debugOptionsWindow;

    public void OnStartup(object? sender, StartupEventArgs e)
    {
        var runType = DetermineRunType(e);

        switch (runType)
        {
            case RunType.Configure:
            {
                MainWindow = new OptionsWindow();
                break;
            }
            case RunType.Show:
            {
                MainWindow = new DisplayWindow(new DisplayWindow.DisplayMode.Screensaver());
                break;
            }
            case RunType.Preview(var parentHandle):
            {
                MainWindow = new DisplayWindow(new DisplayWindow.DisplayMode.Preview(parentHandle));
                break;
            }
            case RunType.Debug(bool withOptions):
            {
                if (withOptions)
                {
                    _debugOptionsWindow = new OptionsWindow(new OptionsSaver().Load());
                    _debugOptionsWindow.Show();
                }

                MainWindow = new DisplayWindow(new DisplayWindow.DisplayMode.Debug(_debugOptionsWindow));

                break;
            }
            case RunType.DebugPaletteCustomizer:
            {
                MainWindow = new PaletteCustomizer();

                break;
            }
            case null:
            {
                Shutdown();
                break;
            }
        }

        MainWindow.Show();
    }

    private RunType? DetermineRunType(StartupEventArgs e)
    {
        var normalizedArgs = e.Args
            .Select(arg => arg.ToLower().Trim())
            .Select(arg => arg.Replace("-", "/"))
            .ToList();

        RunType? runType = normalizedArgs switch
        {
            [] => new RunType.Configure(),
            ["/c"] => new RunType.Configure(),
            ["/d"] => new RunType.Debug(WithOptions: false),
            ["/dd"] => new RunType.Debug(WithOptions: true),
            ["/s"] => new RunType.Show(),
            ["/u"] => new RunType.DebugPaletteCustomizer(),
            ["/s", var handle] => new RunType.Show(),
            ["/p", var handle] => new RunType.Preview(nint.Parse(handle)),

            [var flag] when flag.StartsWith("/s") && flag.Contains(':') => 
                new RunType.Show(),

            [var flag] when flag.StartsWith("/p") && flag.Contains(':') =>
                new RunType.Preview(nint.Parse(flag.Split(':')[1])),

            _ => null,
        };

        return runType;
    }
}
