using System;
using System.Collections.Generic;
using yoksdotnet.data.entities;

namespace yoksdotnet.data;

public class Scene
{
    public int frame = 0;
    public double seconds = 0.0;
    public double lastDtMs = 0.0;
    public DateTimeOffset? lastTick;

    public required Pattern currentPattern;
    public DateTimeOffset? patternLastChangedAt;

    public List<Entity> entities = [];

    public required int width;
    public required int height;
}
