using SkiaSharp;

using System;
using System.Windows;

using yoksdotnet.common;
using Vector = yoksdotnet.common.Vector;

namespace yoksdotnet.drawing;

public static class Bitmaps
{
    public static SKBitmap LoadResource(string path)
    {
        var uri = new Uri(path, UriKind.Relative);
        var resourceStream = Application.GetResourceStream(uri).Stream;
        var resource = SKBitmap.Decode(new SKManagedStream(resourceStream));
        return resource;
    }
}

public class Bitmap 
{ 
    private Bitmap() {}

    public ClassicBitmap? ClassicBitmap { get; init; }
    public RefinedBitmap? RefinedBitmap { get; init; }

    public static Bitmap Classic(ClassicBitmap bitmap) => new()
    {
        ClassicBitmap = bitmap,
    };

    public static Bitmap Refined(RefinedBitmap bitmap) => new()
    {
        RefinedBitmap = bitmap,
    };
}

public class ClassicBitmap : ISfEnum
{
    public static int Size => 128;

    public readonly static ClassicBitmap Lk = new("lk");
    public readonly static ClassicBitmap LkConcern = new("lkconcern");
    public readonly static ClassicBitmap LkCool = new("lkcool");
    public readonly static ClassicBitmap LkExhausted = new("lkexhausted");
    public readonly static ClassicBitmap LkHusk = new("lkhusk");
    public readonly static ClassicBitmap LkJoy = new("lkjoy");
    public readonly static ClassicBitmap LkSix = new("lksix");
    public readonly static ClassicBitmap LkThink = new("lkthink");
    public readonly static ClassicBitmap LkThumbsup = new("lkthumbsup");
    public readonly static ClassicBitmap LkUnamused = new("lkunamused");
    public readonly static ClassicBitmap LkXd = new("lkxd");

    public readonly static ClassicBitmap CvJoy = new("cvjoy");
    public readonly static ClassicBitmap Fn = new("fn");
    public readonly static ClassicBitmap FnPlead = new("fnplead");
    public readonly static ClassicBitmap Nx = new("nx");
    public readonly static ClassicBitmap Vx = new("vx");

    public string Name { get; init; }
    public SKBitmap Resource { get; init; }

    private ClassicBitmap(string name)
    {
        Name = name;
        Resource = Bitmaps.LoadResource($"/resources/sprites/classic/{name}.png");
    }

    public override string ToString() => Name;
}

public class RefinedBitmap : ISfEnum
{
    public static int Size => 128;

    public readonly static RefinedBitmap Neutral = new(
        "neutral",
        pupilCenter: new(58.0, 67.0),
        pupilRange: 10.0
    );

    public string Name { get; init; }
    public SKBitmap Resource { get; init; }
    public Vector PupilCenter { get; init; }
    public double PupilRange { get; init; }

    private RefinedBitmap(string name, Vector pupilCenter, double pupilRange)
    {
        Name = name;
        Resource = Bitmaps.LoadResource($"/resources/sprites/refined/{name}.png");

        PupilCenter = pupilCenter;
        PupilRange = pupilRange;
    }

    public override string ToString() => Name;
}