using yoksdotnet.common;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.data;

public record Pattern(
    string Name,
    string Description,
    PatternSimulator Simulator
) 
    : ISfEnum
{
    public readonly static Pattern Lattice = new(
        "Lattice",
        "No motion",
        new SimpleMovement.LatticeSimulator()
    ); 

    public readonly static Pattern Roamers = new(
        "Roamers", 
        "Drift from left to right, softly bobbing up and down",
        new SimpleMovement.RoamersSimulator()
    );

    public readonly static Pattern Bouncy = new(
        "Bouncy",
        "DVD screensaver style",
        new BouncySimulator()
    );

    public readonly static Pattern Waves = new(
        "Waves", 
        "Move in large, random circles",
        new SimpleMovement.WavesSimulator()
    );

    public readonly static Pattern Lissajous = new(
        "Lissajous",
        "Line up and march in a snaking curve",
        new SimpleMovement.LissajousSimulator()
    );

    public readonly static Pattern Bubbles = new(
        "Bubbles",
        "Become bubbles and bounce off one another",
        new BubblesSimulator()
    );

    public readonly static Pattern Boids = new(
        "Boids",
        "Coalesce and flock",
        new BoidsSimulator()
    );

    public override string ToString() => Name;
}
