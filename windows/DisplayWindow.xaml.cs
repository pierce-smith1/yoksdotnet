using SkiaSharp;
using SkiaSharp.Views.Desktop;

using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using yoksdotnet.drawing;
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

    private EntityPainter _painter = new();

    public record DisplayMode()
    {
        public record Debug() : DisplayMode();
        public record Screensaver() : DisplayMode();
        public record Preview(nint parentHandle) : DisplayMode();
    }

    public DisplayWindow(DisplayMode mode)
    {
        InitializeComponent();

        switch (mode)
        {
            case DisplayMode.Screensaver:
                InitForScreensaver();
                break;

            case DisplayMode.Preview(var parentHandle):
                // Doesn't work and I don't know why. Ignoring for now but we should come back to this.
                break;
        }

        Scene = new
        (
            options: new(),
            width: (int)Width,
            height: (int)Height
        );

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
        e.Surface.Canvas.Clear(new SKColor(0x11, 0x11, 0x11));

        foreach (var entity in Scene?.Entities ?? [])
        {
            _painter?.Draw(e.Surface.Canvas, entity);
        }
    }

    private void CreateGlContext(object? sender, EventArgs e)
    {
        var glControl = new SKGLControl();
        glControl.PaintSurface += OnPaintSurface;
        glControl.Dock = DockStyle.Fill;

        var host = (WindowsFormsHost)sender!;
        host.Child = glControl;
    }
}
