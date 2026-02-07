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

    private readonly int _animationFps = 60;
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
        MainCanvas.Child.MouseMove += (s, e) =>
        {
            if (lastMouseLocation is not null && lastMouseLocation != (e.X, e.Y))
            {
                Shutdown();
            }

            lastMouseLocation = (e.X, e.Y);
        };

        KeyDown += (s, e) => Shutdown();
    }

    private void InitForDebug(OptionsWindow? debugOptionsWindow)
    {
        MainCanvas.Child.MouseUp += (s, e) =>
        {
            var clickedSprite = _ctx.scene.sprites.FirstOrDefault(sprite =>
            {
                var bounds = sprite.Bounds;

                var mouseXIntersects = e.X > bounds.topLeft.X && e.X < bounds.bottomRight.X;
                var mouseYIntersects = e.Y > bounds.topLeft.Y && e.Y < bounds.bottomRight.Y;

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
        var loopTimer = new System.Timers.Timer(1000 / _animationFps);
        loopTimer.Elapsed += OnTick;
        loopTimer.Start();
    }
    
    private void OnTick(object? _sender, ElapsedEventArgs _e)
    {
        SceneSimulation.HandleFrame(_ctx);
        RefreshSurface();
    }

    private void RefreshSurface()
    {
        MainCanvas.Child?.Invalidate();
    }

    private void OnPaintSurface(object? sender, SKPaintGLSurfaceEventArgs e)
    {
        _scenePainter.Draw(e.Surface.Canvas);
    }

    private void CreateGlContext(object? sender, EventArgs e)
    {
        var glControl = new SKGLControl();
        glControl.PaintSurface += OnPaintSurface;
        glControl.Dock = DockStyle.Fill;

        var host = (WindowsFormsHost)sender!;
        host.Child = glControl;
    }

    private void Shutdown()
    {
        System.Windows.Application.Current.Shutdown();
    }
}

