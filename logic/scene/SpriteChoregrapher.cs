using System;
using System.Linq;

using yoksdotnet.common;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.logic.scene;

public class SpriteChoregrapher(Scene scene, ScrOptions options, Random rng)
{
    private readonly EmotionHandler _emotionHandler = new(scene, new(rng));
    private readonly RandomSampler _sampler = new(rng);
    private readonly PatternMover _mover = new(scene);
    private readonly YokinShaker _yokinShaker = new(rng);

    public void HandleFrame()
    {
        if (ShouldChangePattern())
        {
            ChangePattern();
        }

        _mover.MoveByPattern(scene.currentPattern ?? Pattern.Lattice);

        var yokins = scene.sprites.Where(s => s is Yokin).Cast<Yokin>();
        _yokinShaker.ShakeYokins(yokins);
        _emotionHandler.UpdateEmotions(yokins);
    }

    public bool ShouldChangePattern()
    {
        if (!options.AnimationPatternDoesChange)
        {
            return false;
        }

        if (scene.patternLastChangedAt is null)
        {
            scene.patternLastChangedAt = DateTimeOffset.Now;
        }

        var shouldChange = DateTimeOffset.Now > scene.patternLastChangedAt?.AddSeconds(options.GetActualPatternChangeFrequencySeconds());
        return shouldChange;
    }

    public void ChangePattern()
    {
        var possiblePatterns = options.AnimationPossiblePatterns
            .Where(pattern => pattern != scene.currentPattern);

        if (!possiblePatterns.Any())
        {
            return;
        }

        scene.currentPattern = _sampler.Sample(possiblePatterns);
        scene.patternLastChangedAt = DateTimeOffset.Now;
    }
}