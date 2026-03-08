using yoksdotnet.common;

namespace yoksdotnet.data.entities;

public class Trail : EntityComponent
{
    public required CircularBuffer<Entity?> snapshots;
}
