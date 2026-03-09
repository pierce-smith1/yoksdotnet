using SkiaSharp.Views.Desktop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using yoksdotnet.data;
using yoksdotnet.drawing.painters;
using yoksdotnet.logic;
using yoksdotnet.logic.scene;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace yoksdotnet.windows;

public record DisplayMode
{
    public bool IsDebug { get; init; } = default;
    public OptionsWindow? DebugOptionsWindow { get; init; } = default;

    public bool IsStretchedScreensaver { get; init; } = default;
    public Screen? SingleScreen { get; init; } = default;

    public nint? PreviewHwnd { get; init; } = default;
    public bool IsPreview => PreviewHwnd is not null;

    public static DisplayMode Debug(OptionsWindow? optionsWindow) => new()
    {
        IsDebug = true,
        DebugOptionsWindow = optionsWindow,
    };

    public static DisplayMode StretchedScreensaver() => new()
    {
        IsStretchedScreensaver = true,
    };

    public static DisplayMode SingleScreensaver(Screen screen) => new()
    {
        SingleScreen = screen,
    };

    public static DisplayMode Preview(nint hwnd) => new()
    {
        PreviewHwnd = hwnd,
    };

    public void Switch(Action<OptionsWindow?> whenDebug, Action whenStretchedScreensaver, Action<Screen> whenSingleScreensaver, Action<nint> whenPreview)
    {
        if (IsDebug) whenDebug(DebugOptionsWindow);
        if (IsStretchedScreensaver) whenStretchedScreensaver();
        if (SingleScreen is not null) whenSingleScreensaver(SingleScreen);
        if (PreviewHwnd is not null) whenPreview(PreviewHwnd.Value);
    }
}

public partial class DisplayWindow : Window
{
    private static readonly int _fixedRngSeed = new Random().Next();

    private readonly int _animationFps = 60;
    private readonly AnimationContext _ctx;

    private readonly DisplayMode _displayMode;
    private readonly ScenePainter _scenePainter;

    public DisplayWindow(DisplayMode mode)
    {
        InitializeComponent();

        _displayMode = mode;

        mode.Switch(
            whenStretchedScreensaver: InitForStetchedScreensaver,
            whenSingleScreensaver: InitForSingleScreensaver,
            whenDebug: InitForDebug,
            whenPreview: InitForPreview
        );

        var options = GetOptions();
        var rng = GetRng(options);

        if (_displayMode.IsPreview)
        {
            options.spriteScale /= 5.0;
            options.animationSpeed /= 5.0;

            Width /= 2.0;
            Height /= 2.0;
        }

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

    private void InitForStetchedScreensaver()
    {
        InitForScreensaver();

        var globalMinX = Screen.AllScreens.Min(screen => screen.Bounds.Left);
        var globalMaxX = Screen.AllScreens.Max(screen => screen.Bounds.Right);

        var globalMinY = Screen.AllScreens.Min(screen => screen.Bounds.Top);
        var globalMaxY = Screen.AllScreens.Max(screen => screen.Bounds.Bottom);

        Width = globalMaxX - globalMinX;
        Height = globalMaxY - globalMinY;

        Left = 0;
        Top = 0;
    }

    private void InitForSingleScreensaver(Screen screen)
    {
        InitForScreensaver();

        Width = screen.Bounds.Width;
        Height = screen.Bounds.Height;

        Left = screen.Bounds.Left;
        Top = screen.Bounds.Top;
    }

    private void InitForDebug(OptionsWindow? debugOptionsWindow)
    {
        MainCanvas.Child.MouseUp += (s, e) =>
        {
            var clickedSprite = _ctx.scene.entities.FirstOrDefault(entity =>
            {
                var bounds = entity.basis.Bounds;

                var mouseXIntersects = e.X > bounds.topLeft.X && e.X < bounds.bottomRight.X;
                var mouseYIntersects = e.Y > bounds.topLeft.Y && e.Y < bounds.bottomRight.Y;

                return mouseXIntersects && mouseYIntersects;
            });

            _scenePainter.DebuggedEntity = clickedSprite;
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

    private void InitForPreview(nint parentHandle)
    {
        WindowStyle = WindowStyle.None;
        ResizeMode = ResizeMode.NoResize;

        Left = 0;
        Top = 0;

        Loaded += (_s, _e) =>
        {
            var interopHelper = new WindowInteropHelper(this);
            var thisHwnd = (HWND)interopHelper.Handle;

            var parentHwnd = (HWND)parentHandle;

            var setParentResult = PInvoke.SetParent(thisHwnd, parentHwnd);

            var currentWindowStyle = PInvoke.GetWindowLong(thisHwnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
            var setWindowLongResult = PInvoke.SetWindowLong(thisHwnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE, currentWindowStyle | (int)WINDOW_STYLE.WS_CHILD);

            unsafe
            {
                RECT parentRect = new();
                PInvoke.GetClientRect(parentHwnd, &parentRect);

                Width = parentRect.Width;
                Height = parentRect.Height;
            }
        };
    }

    private void RefreshScene()
    {
        var options = GetOptions();
        var rng = GetRng(options);

        var newScene = SceneCreation.NewScene(options, rng, (int)Width, (int)Height);

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
            nameof(OptionsViewModel.IndividualTrailsEnabled),
            nameof(OptionsViewModel.IndividualTrailLength),
            nameof(OptionsViewModel.IndividualUseRefinedStyle)
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

    private Random GetRng(ScrOptions options)
    {
        var debuggingWithOptionsWindow = _displayMode.IsDebug && _displayMode.DebugOptionsWindow is not null;
        if (debuggingWithOptionsWindow)
        {
            return new Random(_fixedRngSeed);
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
        RefreshSurface();
    }

    private void RefreshSurface()
    {
        MainCanvas.Child?.Invalidate();
    }

    private void OnPaintSurface(object? sender, SKPaintGLSurfaceEventArgs e)
    {
        SceneSimulation.HandleFrame(_ctx);
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

    private static void Shutdown()
    {
        System.Windows.Application.Current.Shutdown();
    }
}

