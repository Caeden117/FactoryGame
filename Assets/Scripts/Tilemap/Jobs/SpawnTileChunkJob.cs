using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static TilemapUtils;

[BurstCompile]
public struct SpawnTileChunkJob : IJobParallelFor
{
    public Entity TilePrototype;
    [ReadOnly] public NativeArray<int2> TileCoordinates;
    public EntityCommandBuffer.ParallelWriter Ecb;

    public void Execute(int index)
    {
        // Instantiate our prototype tile entity
        var newTile = Ecb.Instantiate(index, TilePrototype);
        var chunkCoordinate = TileCoordinates[index];

        // Set tile values and transform.
        Ecb.SetComponent(index, newTile, new TilemapChunk(chunkCoordinate));
        Ecb.SetComponent(index, newTile, new LocalToWorld { Value = ComputeLocalTransformMatrix(chunkCoordinate) });
    }

    private static float4x4 ComputeLocalTransformMatrix(int2 chunkCoords)
        => float4x4.Translate(new float3(chunkCoords * CHUNK_SIZE, 0));
}
