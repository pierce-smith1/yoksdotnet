﻿using SkiaSharp;
using SkiaSharp.Views.Desktop;

using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using yoksdotnet.drawing;
using yoksdotnet.logic.scene;

namespace yoksdotnet.windows;

public partial class DisplayWindow : Window
{
    public Scene? Scene { get; private set; }
    public int DesiredFps { get; private set; } = 60;

    private EntityPainter _painter = new();

    public DisplayWindow()
    {
        InitializeComponent();

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
