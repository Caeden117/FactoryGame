using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static TilemapUtils;

/// <summary>
/// ECS Component for a chunk of tilemap data. This includes the chunk's X/Y coordinate in Chunk space, and the tile data it contains.
/// </summary>
public readonly struct TilemapChunk : IComponentData, IDisposable
{
    /// <summary>
    /// X coordinate of this chunk, in chunk-space.
    /// </summary>
    public readonly int ChunkX;
    
    /// <summary>
    /// Y coordinate of this chunk, in chunk-space.
    /// </summary>
    public readonly int ChunkY;

    /// <summary>
    /// Flattened 2D array of tile data associated with this chunk.
    /// </summary>
    // Needs to be this NativeArray container as regular C# arrays are managed types unsupported by ECS.
    public readonly NativeArray<TileData> TilemapData;

    public TilemapChunk(int chunkX, int chunkY)
    {
        ChunkX = chunkX;
        ChunkY = chunkY;
        TilemapData = new NativeArray<TileData>(CHUNK_SIZE * CHUNK_SIZE, Allocator.Persistent);
    }

    /// <summary>
    /// Helper indexer to directly access <see cref="TileData"/> on this chunk.
    /// </summary>
    /// <param name="flattenedIndex">Flattened 1D index of tile data to obtain.</param>
    /// <returns>Read-only reference to the <see cref="TileData"/>.</returns>
    public readonly TileData this[int flattenedIndex] => TilemapData[flattenedIndex];

    /// <summary>
    /// Helper indexer to directly access <see cref="TileData"/> on this chunk.
    /// </summary>
    /// <param name="localX">Local X coordinate of the tile.</param>
    /// <param name="localY">Local Y coordinate of the tile.</param>
    /// <returns>Read-only reference to the <see cref="TileData"/>.</returns>
    public readonly TileData this[int localX, int localY] => TilemapData[(localX * CHUNK_SIZE) + localY];

    /// <summary>
    /// Helper indexer to directly access <see cref="TileData"/> on this chunk.
    /// </summary>
    /// <param name="localCoords">Tile coordinates in local chunk space.</param>
    /// <returns>Read-only reference to the <see cref="TileData"/>.</returns>
    public readonly TileData this[int2 localCoords] => TilemapData[(localCoords.x * CHUNK_SIZE) + localCoords.y];

    // Because we have a NativeArray to handle, we need to implement IDisposable and Dispose() of our NativeArray when we exit.
    public void Dispose() => TilemapData.Dispose();
}
