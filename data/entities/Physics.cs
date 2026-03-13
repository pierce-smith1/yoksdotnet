using yoksdotnet.common;

namespace yoksdotnet.data.entities;

public class Physics : EntityComponent
{
    public required Vector velocity;
}

public class PhysicsMeasurements : EntityComponent
{
    public required Vector lastVelocity;
}

