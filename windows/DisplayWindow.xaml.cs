using SkiaSharp;
using SkiaSharp.Views.Desktop;

using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using yoksdotnet.drawing;
using yoksdotnet.logic;
using yoksdotnet.logic.scene;

namespace yoksdotnet.windows;

public partial class DisplayWindow : Window
{
    public Scene? Scene { get; private set; }
    public int DesiredFps { get; private set; } = 60;

    private EntityPainter _drawer = new();

    public DisplayWindow()
    {
        InitializeComponent();

        WindowState = WindowState.Maximized;

        Scene = new
        (
            options: new(),
            width: (int)Width,
            height: (int)Height
        );

        StartLoop();
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
        e.Surface.Canvas.Clear(SKColors.BlueViolet);

        foreach (var entity in Scene?.GetEntityView() ?? [])
        {
            _drawer?.Draw(e.Surface.Canvas, entity);
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
