using System;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;

using yoksdotnet.windows;

namespace yoksdotnet;

internal record RunType()
{
    internal record Configure() : RunType();
    internal record Show(long? WindowHandle) : RunType();
    internal record Preview(long WindowHandle) : RunType();
    internal record Debug() : RunType();
}

public partial class App : Application
{
    public App() {}

    public void OnStartup(object? sender, StartupEventArgs e)
    {
        var runType = DetermineRunType(e);

        switch (runType)
        {
            case RunType.Configure: 
                throw new NotImplementedException();

            case RunType.Show:
                MainWindow = new DisplayWindow(DisplayWindow.DisplayMode.Screensaver);
                MainWindow.Show();
                break;

            case RunType.Preview(var parentHandle):
                throw new NotImplementedException();

            case RunType.Debug:
                MainWindow = new DisplayWindow(DisplayWindow.DisplayMode.Windowed);
                MainWindow.Show();
                break;

            case null:
                Shutdown();
                break;
        }
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
            ["/d"] => new RunType.Debug(),
            ["/s"] => new RunType.Show(null),
            ["/s", var handle] => new RunType.Show(long.Parse(handle)),
            ["/p", var handle] => new RunType.Preview(long.Parse(handle)),

            [var flag] when flag.StartsWith("/s") && flag.Contains(':') => 
                new RunType.Show(long.Parse(flag.Split(':')[1])),

            [var flag] when flag.StartsWith("/p") && flag.Contains(':') =>
                new RunType.Preview(long.Parse(flag.Split(':')[1])),

            _ => null,
        };

        return runType;
    }
}
