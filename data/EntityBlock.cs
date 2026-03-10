using System.Collections.Generic;
using System.Linq;
using yoksdotnet.data.entities;

namespace yoksdotnet.data;

public class EntityBlock
{
    public required int index;

    public List<Entity> entities = [];
    public List<EntityBlock> neighbors = [];

    public IEnumerable<Entity> AllAround => 
        entities.Concat(neighbors.SelectMany(n => n.entities));
}
