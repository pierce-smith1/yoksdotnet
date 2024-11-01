using SkiaSharp;

using yoksdotnet.logic.scene;

namespace yoksdotnet.drawing;

public static class DrawUtils
{
    public static SKRect GetRect(this Sprite sprite)
    {
        var (TopLeft, BotRight) = sprite.GetBounds();

        var rect = new SKRect((float)TopLeft.X, (float)TopLeft.Y, (float)BotRight.X, (float)BotRight.Y);
        return rect;
    }
}
