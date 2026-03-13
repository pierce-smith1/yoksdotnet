using System.Collections.Generic;
using System.Linq;
using yoksdotnet.common;
using yoksdotnet.data.entities;

namespace yoksdotnet.data;

public class EntityBlock
{
    public static int MaxEntitiesPerBlock = 16;

    public required int index;

    public FixedCircularArray<Entity> entities = new(MaxEntitiesPerBlock);
    public FixedCircularArray<Entity> interactibleEntities = new(MaxEntitiesPerBlock * 9);

    public List<EntityBlock> neighbors = [];
}
