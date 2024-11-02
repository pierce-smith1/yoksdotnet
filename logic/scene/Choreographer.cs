using System.Linq;

using yoksdotnet.common;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.logic.scene;

public class Choreographer
{
    public Scene Scene { get; init; }
    public ScrOptions Options { get; init; }

    private EmotionHandler _emotionHandler;
    private double? _lastPatternChangeSeconds = null;
    private PatternId _currentPattern;

    public Choreographer(ScrOptions options, Scene scene)
    {
        Options = options;
        Scene = scene;

        _emotionHandler = new()
        {
            Scene = scene,
            Options = options,
        };
        _currentPattern = options.StartingPattern ?? RandomUtils.SharedRng.Sample(options.AvailablePatterns);
    }

    public void HandleFrame()
    {
        if (ShouldChangePattern())
        {
            ChangePattern();
        }

        foreach (var sprite in Scene.Sprites)
        {
            Patterns.GetFunction(_currentPattern)(Scene, sprite, Scene.Sprites);

            SpriteMovers.OffscreenMover(Scene, sprite, Scene.Sprites);
            SpriteMovers.YokinShakeMover(Scene, sprite, Scene.Sprites);

            _emotionHandler.UpdateEmotions(sprite);
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