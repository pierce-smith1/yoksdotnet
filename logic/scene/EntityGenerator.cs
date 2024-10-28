using System;
using System.Collections.Generic;

using yoksdotnet.common;
using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public class EntityGenerator
{
    public required ScrOptions Options { get; init; }

    private static int _runningId = 0;

    public IEnumerable<SceneEntity> Make(double spreadX = 1.0, double spreadY = 1.0)
    {
        var rng = RandomUtils.SharedRng;

        List<SceneEntity> entities = new(Options.EntityCount);

        for (var i = 0; i < Options.EntityCount; i++)
        {
            var newEntity = new SceneEntity
            {
                Id = _runningId++,
                Brand = rng.NextDouble(),
                Home = new(rng.NextDouble() * spreadX, rng.NextDouble() * spreadY),
                Offset = new(0, 0),
                Scale = Options.EntityScale,
                Width = 128,
                Height = 128,
                AngleRadians = 0.0,
                Paint = Palettes.Paints[rng.SampleEnum<PaletteId>()],
            };

            entities.Add(newEntity);
        }

        return entities;
    }
}
