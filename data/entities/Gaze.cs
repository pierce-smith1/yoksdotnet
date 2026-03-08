using System;
using yoksdotnet.common;

namespace yoksdotnet.data.entities;

public class Gaze : EntityComponent
{
    public required Vector currentGazePoint;
    public required Vector targetGazePoint;
    public required double targetChangeCooldownSeconds;
    public Entity? targetEntity;
    public DateTimeOffset? lastTargetChange;
}
