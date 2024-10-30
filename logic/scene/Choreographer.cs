using System;
using System.Linq;

using yoksdotnet.common;

namespace yoksdotnet.logic.scene;

public class Choreographer
{
    public Scene Scene { get; init; }
    public ScrOptions Options { get; init; }

    private double? _lastPatternChangeSeconds = null;
    private PatternId _currentPattern;

    public Choreographer(ScrOptions options, Scene scene)
    {
        Options = options;
        Scene = scene;

        _currentPattern = options.StartingPattern ?? RandomUtils.SharedRng.Sample(options.AvailablePatterns);
    }

    public void HandleFrame()
    {
        if (ShouldChangePattern())
        {
            ChangePattern();
        }

        foreach (var entity in Scene.Entities)
        {
            Patterns.Functions[_currentPattern](Scene, entity, Scene.Entities);
            SpriteMovers.OffscreenMover(Scene, entity, Scene.Entities);
        }
    }

    public bool ShouldChangePattern()
    {
        if (Options.PatternChangeSeconds is null)
        {
            return false;
        }

        if (_lastPatternChangeSeconds is null)
        {
            _lastPatternChangeSeconds = Scene.Seconds;
        }

        var shouldChange = Scene.Seconds > _lastPatternChangeSeconds + Options.PatternChangeSeconds;
        return shouldChange;
    }

    public void ChangePattern()
    {
        var possiblePatterns = Options.AvailablePatterns
            .Where(pattern => pattern != _currentPattern);

        if (! possiblePatterns.Any())
        {
            return;
        }

        _currentPattern = RandomUtils.SharedRng.Sample(possiblePatterns);

        _lastPatternChangeSeconds = Scene.Seconds;
    }
}