using SkiaSharp.Views.Desktop;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using yoksdotnet.drawing.painters;
using yoksdotnet.logic;
using yoksdotnet.logic.scene;

namespace yoksdotnet.windows;

public record DisplayMode
{
    public bool IsDebug { get; init; } = default;
    public OptionsWindow? DebugOptionsWindow { get; init; } = default;
    public bool IsScreensaver { get; init; } = default;
    public nint? PreviewHwnd { get; init; } = default;

    public static DisplayMode Debug(OptionsWindow? optionsWindow) => new()
    {
        IsDebug = true,
        DebugOptionsWindow = optionsWindow,
    };

    public static DisplayMode Screensaver() => new()
    {
        IsScreensaver = true,
    };

    public static DisplayMode Preview(nint hwnd) => new()
    {
        PreviewHwnd = hwnd,
    };

    public void Match(Action<OptionsWindow?> whenDebug, Action whenScreensaver, Action<nint> whenPreview)
    {
        if (IsDebug) whenDebug(DebugOptionsWindow);
        if (IsScreensaver) whenScreensaver();
        if (PreviewHwnd is not null) whenPreview(PreviewHwnd.Value);
    }
}

public partial class DisplayWindow : Window
{
    [DllImport("user32.dll")]
    static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

    private readonly AnimationContext _ctx;
    private readonly int _debugRngSeed = new Random().Next();

    private readonly DisplayMode _displayMode;
    private readonly ScenePainter _scenePainter;

    public DisplayWindow(DisplayMode mode)
    {
        InitializeComponent();

        _displayMode = mode;

        mode.Match(
            whenScreensaver: InitForScreensaver,
            whenDebug: InitForDebug,
            whenPreview: hwnd => { /* TODO */ }
        );

        var rng = GetRng();
        var options = GetOptions();

        var scene = SceneCreation.NewScene(options, rng, (int)Width, (int)Height);
        _ctx = new AnimationContext(scene, options, rng);

        _scenePainter = new(_ctx, mode);

        SizeChanged += (s, e) =>
        {
            _ctx.scene.width = (int)e.NewSize.Width;
            _ctx.scene.height = (int)e.NewSize.Height;
        };

        StartLoop();
    }

    private void InitForScreensaver()
    {
        WindowStyle = WindowStyle.None;
        ResizeMode = ResizeMode.NoResize;

        var globalMinX = Screen.AllScreens.Select(screen => screen.Bounds.Left).Min();
        var globalMaxX = Screen.AllScreens.Select(screen => screen.Bounds.Right).Max();

        var globalMinY = Screen.AllScreens.Select(screen => screen.Bounds.Top).Min();
        var globalMaxY = Screen.AllScreens.Select(screen => screen.Bounds.Bottom).Max();

        Width = globalMaxX - globalMinX;
        Height = globalMaxY - globalMinY;

        Left = 0;
        Top = 0;

        System.Windows.Forms.Cursor.Hide();

        // Windows forms repeatedly fires MouseMove events even if you aren't doing shit.
        // I'm forced to manually check that the position of the mouse hasn't changed.
        (int X, int Y)? lastMouseLocation = null;
        MainCanvas.MouseMove += (s, e) =>
        {
            var x = (int)e.GetPosition(MainCanvas).X;
            var y = (int)e.GetPosition(MainCanvas).Y;

            if (lastMouseLocation is not null && lastMouseLocation != (x, y))
            {
                Shutdown();
            }

            lastMouseLocation = (x, y);
        };

        KeyDown += (s, e) => Shutdown();
    }

    private void InitForDebug(OptionsWindow? debugOptionsWindow)
    {
        MainCanvas.MouseUp += (s, e) =>
        {
            var x = e.GetPosition(MainCanvas).X;
            var y = e.GetPosition(MainCanvas).Y;

            var clickedSprite = _ctx.scene.sprites.FirstOrDefault(sprite =>
            {
                var bounds = sprite.Bounds;

                var mouseXIntersects = x > bounds.topLeft.X && x < bounds.bottomRight.X;
                var mouseYIntersects = y > bounds.topLeft.Y && y < bounds.bottomRight.Y;

                return mouseXIntersects && mouseYIntersects;
            });

            _scenePainter.DebuggedSprite = clickedSprite;
        };

        if (debugOptionsWindow is not null)
        {
            var viewModel = debugOptionsWindow.ViewModel;
            var debugOptions = viewModel.BackingOptions;

            viewModel.PropertyChanged += (s, e) =>
            {
                if (PropertyRequiresSceneRefresh(e.PropertyName ?? ""))
                {
                    RefreshScene();
                }
            };
        }
    }

    private void RefreshScene()
    {
        var newScene = SceneCreation.NewScene(GetOptions(), GetRng(), (int)Width, (int)Height);

        _ctx.scene = newScene;
    }

    private static bool PropertyRequiresSceneRefresh(string propertyName)
    {
        List<string> propertiesRequiringRefresh = [
            nameof(OptionsViewModel.FamilyDiversity),
            nameof(OptionsViewModel.FamilySize),
            nameof(OptionsViewModel.FamilyImpostorDensity),
            nameof(OptionsViewModel.FamilyPaletteChoice),
            nameof(OptionsViewModel.IndividualScale),
            nameof(OptionsViewModel.IndividualEmotionScale),
            nameof(OptionsViewModel.AnimationStartingPattern),
        ];

        return propertiesRequiringRefresh.Contains(propertyName);
    }

    private ScrOptions GetOptions()
    { 
        if (_displayMode.IsDebug && _displayMode.DebugOptionsWindow is not null)
        {
            return _displayMode.DebugOptionsWindow.ViewModel.BackingOptions;
        }

        var savedOptions = OptionsStorage.Load();

        if (savedOptions is null)
        {
            var defaultOptions = new ScrOptions();
            OptionsStorage.Save(defaultOptions);
            
            return defaultOptions;
        }

        return savedOptions;
    }

    private Random GetRng()
    {
        if (_displayMode.IsDebug && _displayMode.DebugOptionsWindow is not null)
        {
            return new Random(_debugRngSeed);
        }

        return new Random();
    }

    private void StartLoop()
    {
        MainCanvas.Start(new()
        {
            RenderContinuously = true,
        });
    }
    
    private void OnPaintSurface(TimeSpan _delta)
    {
        SceneSimulation.HandleFrame(_ctx);
    }

    private static void Shutdown()
    {
        System.Windows.Application.Current.Shutdown();
    }
}

