using System;

namespace yoksdotnet.data.entities;

public class Bubble : EntityComponent
{
    public required double radius;
    public bool isFree = false;

    public bool IsVisible { get; private set; } = false;
    public DateTimeOffset LastVisibilityChange { get; private set; } = DateTimeOffset.Now;

    public void SetVisible(bool visible)
    {
        IsVisible = visible;
        LastVisibilityChange = DateTimeOffset.Now;
    }

}

public class Boid : EntityComponent
{
    public required double avoidRadius;
    public required double visionRadius;
    public required double visionTheta;
}

