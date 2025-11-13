using System;

namespace yoksdotnet.logic.scene;

public static class OptionsExtensions
{
    // TODO: These functions need more nuance.
    public static int GetSpriteCount(this ScrOptions options)
    {
        var spriteCount = (int) Math.Round(options.FamilySize * 50);
        return spriteCount;
    }

    public static int GetColorCount(this ScrOptions options)
    {
        var colorCount = (int) Math.Round(options.FamilyDiversity * 8);
        return colorCount;
    }
}
