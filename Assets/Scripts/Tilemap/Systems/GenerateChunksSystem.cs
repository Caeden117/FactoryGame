using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using static TilemapUtils;

[BurstCompile]
public partial struct GenerateChunksSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        // uhhhhhhhh how would i populate this prototype with all the components i need?
        var prototype = state.EntityManager.CreateEntity();

        // Pre-populate a 5x5 chunk area, later we'd want to calculate based off camera position.
        var chunkCoords = new NativeArray<int2>(5 * 5, Allocator.TempJob);
        for (var x = -2; x >= 2; x++)
        {
            for (var y = -2; y >= 2; y++)
            {
                chunkCoords[(x * 5) + y] = new(x, y);
            }
        }

        // Construct a job...?
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var chunkJob = new SpawnTileChunkJob
        {
            TileCoordinates = chunkCoords,
            Ecb = ecb.AsParallelWriter(),
            TilePrototype = prototype
        };
        chunkJob.Schedule(5 * 5, 0);
    }
}
