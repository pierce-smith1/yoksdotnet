using SkiaSharp.Views.Desktop;

using System;
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

public partial class DisplayWindow : Window
{
    #region Win32 interop defs

    [DllImport("user32.dll")]
    static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

    #endregion

    public Scene? Scene { get; private set; }
    public int DesiredFps { get; private set; } = 60;

    private DisplayMode _displayMode;
    private ScenePainter _scenePainter;

    public DisplayWindow(DisplayMode mode)
    {
        InitializeComponent();

        _displayMode = mode;
        switch (_displayMode)
        {
            case DisplayMode.Screensaver:
                InitForScreensaver();
                break;

            case DisplayMode.Preview(var parentHandle):
                // Doesn't work and I don't know why. Ignoring for now but we should come back to this.
                break;

            case DisplayMode.Debug(OptionsWindow optionsWindow):
                InitForDebug(optionsWindow);
                break;
        }

        Scene = new
        (
            options: GetOptions(),
            width: (int)Width,
            height: (int)Height
        );

        _scenePainter = new()
        {
            Scene = Scene,
            DisplayMode = mode,
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
            var clickedSprite = Scene?.Sprites?.FirstOrDefault(sprite =>
            {
                var bounds = sprite.GetBounds();

                var mouseXIntersects = e.X > bounds.TopLeft.X && e.X < bounds.BotRight.X;
                var mouseYIntersects = e.Y > bounds.TopLeft.Y && e.Y < bounds.BotRight.Y;

                return mouseXIntersects && mouseYIntersects;
            });

            _scenePainter.DebuggedSprite = clickedSprite;
        };

        if (debugOptionsWindow is not null)
        {
            var rngSeed = Guid.NewGuid().GetHashCode();

            var viewModel = debugOptionsWindow.ViewModel;
            var debugOptions = viewModel.BackingOptions;

            viewModel.PropertyChanged += (s, e) =>
            {
                if (Scene is not null && ScrOptions.PropertyRequiresSceneRefresh(e.PropertyName ?? ""))
                {
                    Scene.Refresh(debugOptions, (int)Width, (int)Height, rngSeed);
                }
            };
        }
    }

    private ScrOptions GetOptions()
    { 
        if (_displayMode is DisplayMode.Debug debugMode && debugMode.DebugOptionsWindow is not null)
        {
            return debugMode.DebugOptionsWindow.ViewModel.BackingOptions;
        }

        var savedOptions = _optionsSaver.Load();

        if (savedOptions is null)
        {
            var defaultOptions = new ScrOptions();
            _optionsSaver.Save(defaultOptions);
            
            return defaultOptions;
        }

        return savedOptions;
    }

    private void StartLoop()
    {
        var loopTimer = new System.Timers.Timer(1000 / DesiredFps);
        loopTimer.Elapsed += OnTick;
        loopTimer.Start();
    }
    
    private void OnTick(object? sender, ElapsedEventArgs e)
    {
        Scene?.TickFrame();
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

public record DisplayMode()
{
    public record Debug(OptionsWindow? DebugOptionsWindow) : DisplayMode();
    public record Screensaver() : DisplayMode();
    public record Preview(nint parentHandle) : DisplayMode();
}

