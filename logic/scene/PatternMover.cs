using System;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.logic.scene;

public class PatternMover(Scene scene)
{
    public void MoveByPattern(Pattern pattern)
    {
        if (pattern == Pattern.Roamers) 
        {
            MoveRoamers();
        }
        else if (pattern == Pattern.Waves)
        {
            MoveWaves();
        }
    }

    public void MoveRoamers()
    {
        foreach (var sprite in scene.sprites)
        {
            sprite.home.X += sprite.brand * scene.lastDtMs;
            sprite.home.Y += Math.Sin(scene.seconds * sprite.brand) * scene.lastDtMs;

            WrapSprite(sprite);
        }
    }

    public void MoveWaves()
    {
        foreach (var sprite in scene.sprites)
        {
            sprite.home.X += Math.Sin(scene.seconds * sprite.brand) * scene.lastDtMs;
            sprite.home.Y += Math.Cos(scene.seconds * sprite.brand) * scene.lastDtMs;

            WrapSprite(sprite);
        }
    }

    private void WrapSprite(Sprite sprite)
    {
        var bounds = sprite.Bounds;

        var xInBounds = bounds.bottomRight.X >= 0 && bounds.topLeft.X <= scene.width;
        var yInBounds = bounds.bottomRight.Y >= 0 && bounds.topLeft.Y <= scene.height;

        if (!yInBounds || !xInBounds)
        {
            return;
        }

        var boundsWidth = bounds.bottomRight.X - bounds.topLeft.X;
        var boundsHeight = bounds.bottomRight.Y - bounds.topLeft.Y;

        if (!xInBounds)
        {
            sprite.home.X += (scene.width + boundsWidth) * (sprite.home.X < 0 ? 1 : -1);
        } 

        if (!yInBounds)
        {
            sprite.home.Y += (scene.height + boundsHeight) * (sprite.home.Y < 0 ? 1 : -1);
        }
    }

    private void ClampSprite(Sprite sprite)
    {
        sprite.home.X = Math.Clamp(sprite.home.X, sprite.offset.X, scene.width + sprite.offset.X);
        sprite.home.Y = Math.Clamp(sprite.home.Y, sprite.offset.Y, scene.height + sprite.offset.Y);
    }
}
