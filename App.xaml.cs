using System;
using System.Linq;
using System.Windows;

using yoksdotnet.logic;
using yoksdotnet.windows;

namespace yoksdotnet;

public record RunType;
public record RunAsConfigure : RunType;
public record RunAsScreensaver : RunType;
public record RunAsPreview(nint WindowHandle) : RunType;
public record RunAsDebug(bool WithOptions) : RunType;

public partial class App : Application
{
    public App() {}

    private OptionsWindow? _debugOptionsWindow;

    public void OnStartup(object? sender, StartupEventArgs e)
    {
        var runType = DetermineRunType(e);

        switch (runType)
        {
            case RunAsConfigure:
            {
                MainWindow = new OptionsWindow();
                break;
            }
            case RunAsScreensaver:
            {
                MainWindow = new DisplayWindow(new DisplayMode.Screensaver());
                break;
            }
            case RunAsPreview(var parentHandle):
            {
                MainWindow = new DisplayWindow(new DisplayMode.Preview(parentHandle));
                break;
            }
            case RunAsDebug(bool withOptions):
            {
                if (withOptions)
                {
                    _debugOptionsWindow = new OptionsWindow(OptionsStorage.Load());
                    _debugOptionsWindow.Show();
                }

                MainWindow = new DisplayWindow(new DisplayMode.Debug(_debugOptionsWindow));

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
            [] => new RunAsConfigure(),
            ["/c"] => new RunAsConfigure(),
            ["/d"] => new RunAsDebug(WithOptions: false),
            ["/dd"] => new RunAsDebug(WithOptions: true),
            ["/s"] => new RunAsScreensaver(),
            ["/s", var handle] => new RunAsScreensaver(),
            ["/p", var handle] => new RunAsPreview(nint.Parse(handle)),

            [var flag] when flag.StartsWith("/s") && flag.Contains(':') => 
                new RunAsScreensaver(),

            [var flag] when flag.StartsWith("/p") && flag.Contains(':') =>
                new RunAsPreview(nint.Parse(flag.Split(':')[1])),

            _ => null,
        };

        return runType;
    }
}
