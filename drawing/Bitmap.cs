using SkiaSharp;

using System;
using System.Windows;

using yoksdotnet.common;

namespace yoksdotnet.drawing;

public class Bitmap : ISfe
{
    public static int BitmapSize() => 128;

    public readonly static Bitmap Lk = new("lk");
    public readonly static Bitmap LkConcern = new("lkconcern");
    public readonly static Bitmap LkCool = new("lkcool");
    public readonly static Bitmap LkExhausted = new("lkexhausted");
    public readonly static Bitmap LkHusk = new("lkhusk");
    public readonly static Bitmap LkJoy = new("lkjoy");
    public readonly static Bitmap LkSix = new("lksix");
    public readonly static Bitmap LkThink = new("lkthink");
    public readonly static Bitmap LkThumbsup = new("lkthumbsup");
    public readonly static Bitmap LkUnamused = new("lkunamused");
    public readonly static Bitmap LkXd = new("lkxd");

    public readonly static Bitmap CvJoy = new("cvjoy");
    public readonly static Bitmap Fn = new("fn");
    public readonly static Bitmap FnPlead = new("fnplead");
    public readonly static Bitmap Nx = new("nx");
    public readonly static Bitmap Vx = new("vx");

    public string Name { get; init; }
    public SKBitmap Resource { get; init; }

    private Bitmap(string name)
    {
        Name = name;
        Resource = LoadResource($"/resources/sprites/{name}.png");
    }

    private static SKBitmap LoadResource(string path)
    {
        var uri = new Uri(path, UriKind.Relative);
        var resourceStream = Application.GetResourceStream(uri).Stream;
        var resource = SKBitmap.Decode(new SKManagedStream(resourceStream));
        return resource;
    }

    public override string ToString() => Name;
}