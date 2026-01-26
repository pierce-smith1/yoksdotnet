using System;
using System.Collections.Generic;

using yoksdotnet.common;

namespace yoksdotnet.logic.scene;

public class Scene
{
    public int Frame { get; private set; }
    public double Seconds { get; private set; } = 0.0;
    public double LastDtMs { get; private set; } = 0.0;

    public ScrOptions Options { get; set; }

    public int Width { get; private set; }
    public int Height { get; private set; }

    public List<Sprite> Sprites { get; private set; } = [];

    private DateTimeOffset? _lastTick;
    private SpriteGenerator? _spriteGenerator;
    private Choreographer? _choreographer;

    public Scene(ScrOptions options, int width, int height)
    {
        Options = options;
        Refresh(options, width, height, Guid.NewGuid().GetHashCode());
    }

    public void Refresh(ScrOptions options, int width, int height, int rngSeed)
    {
        Options = options;

        SetSize(width, height);

        RandomUtils.SeedSharedRng(rngSeed);

        _spriteGenerator = new(Options, RandomUtils.SharedRng);

        _choreographer = new(options: options, scene: this);

        var initialEntities = _spriteGenerator.Make
        (
            spreadX: width,
            spreadY: height
        );
        
        Sprites.Clear();
        Sprites.AddRange(initialEntities);
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

            LastDtMs = deltaTime.Value.TotalMilliseconds * Options.GetActualAnimationSpeedScale();
            Seconds += deltaTime.Value.TotalSeconds * Options.GetActualAnimationSpeedScale();
        }

        _lastTick = now;

        Simulate();
    }

    private void Simulate()
    {
        _choreographer?.HandleFrame();
    }
}
