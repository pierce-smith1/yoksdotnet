using System;
using System.Collections.Generic;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.logic.scene;

public class Scene
{
    public int frame = 0;
    public double seconds = 0.0;
    public double lastDtMs = 0.0;
    public DateTimeOffset? lastTick = null;
    public Pattern? currentPattern = null;
    public DateTimeOffset? patternLastChangedAt = null;

    public List<Entity> entities = [];

    public required int width;
    public required int height;
}
