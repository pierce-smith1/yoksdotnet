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
    private Pattern _currentPattern;

    public Choreographer(ScrOptions options, Scene scene)
    {
        Options = options;
        Scene = scene;

        _emotionHandler = new()
        {
            Scene = scene,
            Options = options,
        };

        _currentPattern = options.AnimationStartingPattern switch
        {
            PatternChoice.Random => RandomUtils.SharedRng.Sample(options.AnimationPossiblePatterns),
            PatternChoice.SinglePattern(Pattern p) => p,

            _ => throw new System.NotImplementedException(),
        };
    }

    public void HandleFrame()
    {
        if (ShouldChangePattern())
        {
            ChangePattern();
        }

        foreach (var sprite in Scene.Sprites)
        {
            _currentPattern.Move(Scene, sprite, Scene.Sprites);

            SpriteMovers.OffscreenMover(Scene, sprite, Scene.Sprites);
            SpriteMovers.YokinShakeMover(Options, Scene, sprite, Scene.Sprites);

            _emotionHandler.UpdateEmotions(sprite);
        }
    }

    public bool ShouldChangePattern()
    {
        if (!Options.AnimationPatternDoesChange)
        {
            return false;
        }

        if (_lastPatternChangeSeconds is null)
        {
            _lastPatternChangeSeconds = Scene.Seconds;
        }

        var shouldChange = Scene.Seconds > _lastPatternChangeSeconds + Options.AnimationPatternChangeFrequency;
        return shouldChange;
    }

    public void ChangePattern()
    {
        var possiblePatterns = Options.AnimationPossiblePatterns
            .Where(pattern => pattern != _currentPattern);

        if (! possiblePatterns.Any())
        {
            return;
        }

        _currentPattern = RandomUtils.SharedRng.Sample(possiblePatterns);

        _lastPatternChangeSeconds = Scene.Seconds;
    }
}