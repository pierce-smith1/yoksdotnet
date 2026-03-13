using System;
using System.Collections.Generic;
using yoksdotnet.common;
using yoksdotnet.data;
using yoksdotnet.data.entities;

namespace yoksdotnet.logic.scene;

public static class EntityBlockMapper
{
    public static List<EntityBlock> InitBlocks(Scene scene, double blockSize)   
    {
        var blocks = new List<EntityBlock>();

        var numBlocksX = (int)Math.Ceiling(scene.width / blockSize);
        var numBlocksY = (int)Math.Ceiling(scene.height / blockSize);

        var numBlocks = numBlocksX * numBlocksY;

        for (var i = 0; i < numBlocks; i++)
        {
            blocks.Add(new EntityBlock
            {
                index = i,
            });
        }

        for (var i = 0; i < numBlocks; i++)
        {
            foreach (var iNeighbor in AdjacentBlockIndices(i, numBlocksX, numBlocksY))
            {
                blocks[i].neighbors.Add(blocks[iNeighbor]);
            }
        }

        return blocks;
    }

    public static void AssignEntities(Scene scene, List<EntityBlock> entityBlocks, IEnumerable<Entity> entities, double blockSize)
    {
        foreach (var block in entityBlocks)
        {
            block.entities.Clear();
            block.interactibleEntities.Clear();
        }

        foreach (var entity in entities)
        {
            var iBlock = CoordToBlockIndex(scene, entity.basis.home, blockSize);
            var block = entityBlocks[iBlock];

            entity.block = block;
            block.entities.Add(entity);
            block.interactibleEntities.Add(entity);

            foreach (var neighbor in block.neighbors)
            {
                neighbor.interactibleEntities.Add(entity);
            }
        }
    }

    public static int CoordToBlockIndex(Scene scene, Vector coord, double blockSize)
    {
        var ix = (int)Math.Floor(coord.X / blockSize);
        var iy = (int)Math.Floor(coord.Y / blockSize);

        var numBlocksX = (int)Math.Ceiling(scene.width / blockSize);
        var numBlocksY = (int)Math.Ceiling(scene.height / blockSize);

        ix = Math.Clamp(ix, 0, numBlocksX - 1);
        iy = Math.Clamp(iy, 0, numBlocksY - 1);

        return ix + (iy * numBlocksX);
    }

    private static IEnumerable<int> AdjacentBlockIndices(int blockIndex, int numBlocksX, int numBlocksY)
    {
        var iTopLeft = blockIndex - numBlocksX - 1;
        var iTop = blockIndex - numBlocksX;
        var iTopRight = blockIndex - numBlocksX + 1;
        var iLeft = blockIndex - 1;
        var iRight = blockIndex + 1;
        var iBotLeft = blockIndex + numBlocksX - 1;
        var iBot = blockIndex + numBlocksX;
        var iBotRight = blockIndex + numBlocksX + 1;

        // Left edge
        if (blockIndex % numBlocksX == 0)
        {
            iTopLeft += numBlocksX;
            iLeft += numBlocksX;
            iBotLeft += numBlocksX;
        }

        // Right edge
        if (blockIndex % numBlocksX == numBlocksX - 1)
        {
            iTopRight -= numBlocksX;
            iRight -= numBlocksX;
            iBotRight -= numBlocksX;
        }

        // Top edge
        if (blockIndex / numBlocksX == 0)
        {
            iTopLeft += numBlocksY * numBlocksX;
            iTop += numBlocksY * numBlocksX;
            iTopRight += numBlocksY * numBlocksX;
        }

        // Bottom edge
        if (blockIndex / numBlocksX == numBlocksY - 1)
        {
            iBotLeft -= numBlocksY * numBlocksX;
            iBot -= numBlocksY * numBlocksX;
            iBotRight -= numBlocksY * numBlocksX;
        }

        return [iTopLeft, iTop, iTopRight, iLeft, iRight, iBotLeft, iBot, iBotRight];
    }
}
