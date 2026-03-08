using System;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.data.entities;

public abstract class EntityComponent;

public class Entity
{
    public Guid Id { get; } = Guid.NewGuid();

    public required Basis basis;
    public required double brand;

    public Emotion? emotion;

    public Skin? skin;
    public Gaze? gaze;
    public Trail? trail;

    public Physics? physics;
    public PhysicsMeasurements? physicsMeasurements;

    public IPatternToken? patternToken;

    public Bubble? bubble;
    public Boid? boid;
}
