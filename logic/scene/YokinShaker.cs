using System;
using System.Collections.Generic;

namespace yoksdotnet.logic.scene;

public class YokinShaker(Random rng)
{
    public void ShakeYokins(IEnumerable<Yokin> yokins)
    {
        foreach (var yokin in yokins)
        {
            var emotionMagnitude = yokin.emotions.Magnitude;

            yokin.offset.X += (rng.NextDouble() * 2.0 - 1.0) * emotionMagnitude * emotionMagnitude;
            yokin.offset.Y += (rng.NextDouble() * 2.0 - 1.0) * emotionMagnitude * emotionMagnitude;

            yokin.offset.X = Math.Clamp(yokin.offset.X, -20.0, 20.0);
            yokin.offset.Y = Math.Clamp(yokin.offset.Y, -20.0, 20.0);
        }
    }
}
