using SkiaSharp;

using System;
using System.Windows;

namespace yoksdotnet.drawing;

public enum BitmapId
{
    Lk,
    LkConcern,
    LkCool,
    LkExhausted,
    LkHusk,
    LkJoy,
    LkSix,
    LkThink,
    LkThumbsup,
    LkUnamused,
    LkXd,

    CvJoy,
    Fn,
    FnPlead,
    Nx,
    Vx,
}

public static class Bitmaps
{
    public readonly static SKBitmap Lk = LoadResource("/resources/lk.png");
    public readonly static SKBitmap LkConcern = LoadResource("/resources/lkconcern.png");
    public readonly static SKBitmap LkCool = LoadResource("/resources/lkcool.png");
    public readonly static SKBitmap LkExhausted = LoadResource("/resources/lkexhausted.png");
    public readonly static SKBitmap LkHusk = LoadResource("/resources/lkhusk.png");
    public readonly static SKBitmap LkJoy = LoadResource("/resources/lkjoy.png");
    public readonly static SKBitmap LkSix = LoadResource("/resources/lksix.png");
    public readonly static SKBitmap LkThink = LoadResource("/resources/lkthink.png");
    public readonly static SKBitmap LkThumbsup = LoadResource("/resources/lkthumbsup.png");
    public readonly static SKBitmap LkUnamused = LoadResource("/resources/lkunamused.png");
    public readonly static SKBitmap LkXd = LoadResource("/resources/lkxd.png");

    public readonly static SKBitmap CvJoy = LoadResource("/resources/cvjoy.png");
    public readonly static SKBitmap Fn = LoadResource("/resources/fn.png");
    public readonly static SKBitmap FnPlead = LoadResource("/resources/fnplead.png");
    public readonly static SKBitmap Nx = LoadResource("/resources/nx.png");
    public readonly static SKBitmap Vx = LoadResource("/resources/vx.png");

    public static SKBitmap Get(BitmapId bitmapId)
    {
        var result = bitmapId switch
        {
            BitmapId.Lk => Lk,
            BitmapId.LkConcern => LkConcern,
            BitmapId.LkCool => LkCool,
            BitmapId.LkExhausted => LkExhausted,
            BitmapId.LkHusk => LkHusk,
            BitmapId.LkJoy => LkJoy,
            BitmapId.LkSix => LkSix,
            BitmapId.LkThink => LkThink,
            BitmapId.LkThumbsup => LkThumbsup,
            BitmapId.LkUnamused => LkUnamused,
            BitmapId.LkXd => LkXd,

            BitmapId.CvJoy => CvJoy,
            BitmapId.Fn => Fn,
            BitmapId.FnPlead => FnPlead,
            BitmapId.Nx => Nx,
            BitmapId.Vx => Vx,

            // :(
            _ => throw new NotImplementedException(),
        };
        return result;
    }

    private static SKBitmap LoadResource(string path)
    {
        var uri = new Uri(path, UriKind.Relative);
        var resourceStream = Application.GetResourceStream(uri).Stream;
        var resource = SKBitmap.Decode(new SKManagedStream(resourceStream));
        return resource;
    }
}
