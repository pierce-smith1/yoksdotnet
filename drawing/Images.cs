using SkiaSharp;

using System;
using System.Windows;

namespace yoksdotnet.drawing;

public static class Images
{
    public readonly static SKBitmap lk = LoadResource("/resources/lk.png");

    private static SKBitmap LoadResource(string path)
    {
        var uri = new Uri(path, UriKind.Relative);
        var resourceStream = Application.GetResourceStream(uri).Stream;
        var resource = SKBitmap.Decode(new SKManagedStream(resourceStream));
        return resource;
    }
}
