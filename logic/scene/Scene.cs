using System;
using System.Collections.Generic;
using System.Windows.Forms;

using yoksdotnet.common;

namespace yoksdotnet.logic.scene;

public class Scene
{
    public int Frame { get; private set; }
    public double Seconds { get; private set; } = 0.0;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public ScrOptions Options { get; init; }

    public List<Sprite> Entities { get; private set; } = [];

    private DateTimeOffset? _lastTick;
    private SpriteGenerator _entityGenerator;
    private Choreographer _choreographer;

    public Scene(ScrOptions options, int width, int height)
    {
        Options = options;
        SetSize(width, height);

        _entityGenerator = new()
        {
            Options = options,
        };

        _choreographer = new(options: options, scene: this);

        var initialEntities = _entityGenerator.Make
        (
            spreadX: width,
            spreadY: height
        );
        Entities.AddRange(initialEntities);
    }

    public void SetSize(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public void TickFrame()
    {
        Frame++;

        var now = DateTimeOffset.Now;

        if (_lastTick is not null)
        {
            var deltaTime = now - _lastTick;
            Seconds += deltaTime.Value.TotalSeconds;
        }

        _lastTick = now;

        Simulate();
    }

    private void Simulate()
    {
        _choreographer.HandleFrame();
    }
}
