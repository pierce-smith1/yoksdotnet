using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace yoksdotnet.logic.scene;

public class Scene
{
    public int Frame { get; private set; }
    public double Seconds { get; private set; } = 0.0;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public ScrOptions Options { get; init; }

    private List<SceneEntity> _entities = [];
    private DateTimeOffset? _lastTick;
    private EntityGenerator _entityGenerator;
    private List<MoveFunction> _movers = [];

    public Scene(ScrOptions options, int width, int height)
    {
        Options = options;
        SetSize(width, height);

        _entityGenerator = new()
        { 
            Options = options,
        };

        var initialEntities = _entityGenerator.Make
        (
            spreadX: width,
            spreadY: height
        );
        _entities.AddRange(initialEntities);

        _movers =
        [
            EntityMovers.Functions[PatternId.Roamers],
            EntityMovers.OffscreenMover,
        ];
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
        foreach (var entity in _entities)
        {
            foreach (var mover in _movers)
            {
                mover(this, entity, _entities);
            }
        }
    }

    public IEnumerable<SceneEntity> GetEntityView()
    {
        return _entities;
    }
}
