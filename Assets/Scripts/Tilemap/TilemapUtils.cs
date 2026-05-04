using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;

/// <summary>
/// Utility class for Tilemap-related operations
/// </summary>
public static class TilemapUtils
{
    /// <summary>
    /// Size of a chunk.
    /// </summary>
    public const int CHUNK_SIZE = 32;

    /// <summary>
    /// All currently loaded chunks in the system.
    /// </summary>
    public static NativeHashMap<int2, TilemapChunk> LoadedChunks;

    /// <summary>
    /// Given a tile coordiante in world-space, calculate the coordinate of the tile local to its containing chunk.
    /// </summary>
    /// <remarks>
    /// This does not return the chunk that contains this tile.
    /// Use in tandem with <see cref="WorldTileToChunkCoord(int2)"/> to grab the chunk coordinate of a world-space tile.
    /// </remarks>
    /// <param name="worldTile">World-space tile coordinate.</param>
    /// <returns>Local-space tile coordinate.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int2 WorldTileToLocalTile(int2 worldTile) => worldTile % CHUNK_SIZE;

    /// <summary>
    /// Given a tile coordinate in world-space, calculate the coordinate of the chunk that contains this tile.
    /// </summary>
    /// <remarks>
    /// This does not return the local-space coordinate of this tile inside the chunk.
    /// Use in tandem with <see cref="WorldTileToLocalTile(int2)"/> to grab the local-space coordinate of the tile.
    /// </remarks>
    /// <param name="worldTile">World-space tile coordinate.</param>
    /// <returns>Chunk coordinate.</returns>
    // TODO: We do need a way to grab a TilemapChunk component via its chunk coordinate.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int2 WorldTileToChunkCoord(int2 worldTile) => worldTile / CHUNK_SIZE;

    /// <summary>
    /// Given a tile coordinate in local-space, and the chunk coordinate of its containing chunk, calculate the world-space tile coordinate.
    /// </summary>
    /// <param name="localTile">Local-space tile coordinate.</param>
    /// <param name="chunkCoord">Chunk coordinate.</param>
    /// <returns>World-space tile coordinate.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int2 LocalTileToWorldTile(int2 localTile, int2 chunkCoord) => (chunkCoord * CHUNK_SIZE) + localTile;
}
