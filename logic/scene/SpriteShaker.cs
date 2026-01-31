using System;
using System.Collections.Generic;

namespace yoksdotnet.logic.scene;

public class SpriteShaker(Random rng)
{
    public void ApplyEmotionShake(IEnumerable<Sprite> sprites)
    {
        foreach (var sprite in sprites)
        {
            if (sprite.addons.emotions is not { } emotions)
            {
                continue;
            }

            sprite.offset.X += (rng.NextDouble() * 2.0 - 1.0) * emotions.Magnitude * emotions.Magnitude;
            sprite.offset.Y += (rng.NextDouble() * 2.0 - 1.0) * emotions.Magnitude * emotions.Magnitude;

            sprite.offset.X = Math.Clamp(sprite.offset.X, -20.0, 20.0);
            sprite.offset.Y = Math.Clamp(sprite.offset.Y, -20.0, 20.0);
        }
    }
}
