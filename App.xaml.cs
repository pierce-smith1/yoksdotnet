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
    public bool? DebugWithOptions { get; init; } = default;

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

    public static RunType RunAsDebug(bool withOptions) => new()
    {
        DebugWithOptions = withOptions,
    };

    public T Match<T>(Func<T> whenConfigure, Func<T> whenScreensaver, Func<nint, T> whenPreview, Func<bool, T> whenDebug)
    {
        if (Configure) return whenConfigure();
        if (Screensaver) return whenScreensaver();
        if (PreviewHwnd is not null) return whenPreview(PreviewHwnd.Value);
        if (DebugWithOptions is not null) return whenDebug(DebugWithOptions.Value);

        throw new NotImplementedException();
    }
}

public partial class App : System.Windows.Application
{
    public App() {}

    private OptionsWindow? _debugOptionsWindow;

    public void OnStartup(object? sender, StartupEventArgs e)
    {
        var runType = DetermineRunType(e);

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
            whenDebug: withOptions =>
            {
                _debugOptionsWindow = new OptionsWindow(options);
                var displayWindow = new DisplayWindow(DisplayMode.Debug(_debugOptionsWindow));

                if (withOptions)
                {
                    return [displayWindow, _debugOptionsWindow];
                }

                return [displayWindow];
            }
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
            ["/d"] => RunType.RunAsDebug(withOptions: false),
            ["/dd"] => RunType.RunAsDebug(withOptions: true),
            ["/s"] => RunType.RunAsScreensaver(),
            ["/s", var handle] => RunType.RunAsScreensaver(),
            ["/p", var handle] => RunType.RunAsPreview(nint.Parse(handle)),

            [var flag] when flag.StartsWith("/s") && flag.Contains(':') 
                => RunType.RunAsScreensaver(),

            [var flag] when flag.StartsWith("/p") && flag.Contains(':')
                => RunType.RunAsPreview(nint.Parse(flag.Split(':')[1])),

            _ => null,
        }; 

        return runType;
    }
}
