using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using yoksdotnet.data;
using yoksdotnet.logic;
using yoksdotnet.windows;

namespace yoksdotnet;

public record RunType
{
    public bool Configure { get; init; } = default;
    public bool Screensaver { get; init; } = default;
    public nint? PreviewHwnd { get; init; } = default;
    public bool Debug { get; init; } = default;

    public static RunType RunAsConfigure() => new()
    {
        Configure = true,
    };

    public static RunType RunAsScreensaver() => new()
    {
        Screensaver = true,
    };

    public static RunType RunAsPreview(nint hwnd) => new()
    {
        PreviewHwnd = hwnd,
    };

    public static RunType RunAsDebug() => new()
    {
        Debug = true,
    };

    public T Match<T>(Func<T> whenConfigure, Func<T> whenScreensaver, Func<nint, T> whenPreview, Func<T> whenDebug)
    {
        if (Configure) return whenConfigure();
        if (Screensaver) return whenScreensaver();
        if (PreviewHwnd is not null) return whenPreview(PreviewHwnd.Value);
        if (Debug) return whenDebug();

        throw new NotImplementedException();
    }
}

public partial class App : System.Windows.Application
{
    public App() {}

    private void OnStartup(object? sender, StartupEventArgs e)
    {
        Logging.Setup();

        Log.Information("yoksdotnet started");

        var runType = DetermineRunType(e);
        Log.Information("Run type is {RunType}", runType);

        if (runType == null)
        {
            Shutdown();
            return;
        }

        var options = OptionsStorage.Load() ?? new ScrOptions();

        var windows = runType.Match<IEnumerable<Window>>(
            whenConfigure: () => [new OptionsWindow()],
            whenScreensaver: () => options.multiMonitorMode.Match(
                whenPerScreen: () => Screen.AllScreens.Select(s => new DisplayWindow(DisplayMode.SingleScreensaver(s))),
                whenStretch: () => [new DisplayWindow(DisplayMode.StretchedScreensaver())]
            ),
            whenPreview: hwnd => [new DisplayWindow(DisplayMode.Preview(hwnd))],
            whenDebug: () => [new OptionsWindow(startRealtimeDebug: true)]
        );

        MainWindow = windows.FirstOrDefault();

        foreach (var window in windows)
        {
            window.Show();
        }
    }

    private static RunType? DetermineRunType(StartupEventArgs e)
    {
        var normalizedArgs = e.Args
            .Select(arg => arg.ToLower().Trim())
            .Select(arg => arg.Replace("-", "/"))
            .ToList();

        RunType? runType = normalizedArgs switch
        {
            [] => RunType.RunAsConfigure(),
            ["/c"] => RunType.RunAsConfigure(),
            ["/d"] => RunType.RunAsDebug(),
            ["/s"] => RunType.RunAsScreensaver(),
            ["/s", var handle] => RunType.RunAsScreensaver(),
            ["/p", var handle] => RunType.RunAsPreview(nint.Parse(handle)),

            [var flag] when flag.StartsWith("/s") && flag.Contains(':') 
                => RunType.RunAsScreensaver(),

            [var flag] when flag.StartsWith("/p") && flag.Contains(':')
                => RunType.RunAsPreview(nint.Parse(flag.Split(':')[1])),

            [var flag] when flag.StartsWith("/c")
                => RunType.RunAsConfigure(),

            _ => null,
        }; 

        return runType;
    }

    private void OnShutdown(object? _sender, ExitEventArgs _e)
    {
        Log.CloseAndFlush();
    }
}
