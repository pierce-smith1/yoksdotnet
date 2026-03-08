using System;
using yoksdotnet.drawing;
using yoksdotnet.logic;

namespace yoksdotnet.data;

public class AnimationContext(Scene scene, ScrOptions options, Random rng)
{
    public Scene scene = scene;
    public ScrOptions options = options;
    public Random rng = rng;

    public PerlinNoiseGenerator noiseGenerator = new(rng);
    public RandomPaletteGenerator paletteGenerator = new(rng);
}
