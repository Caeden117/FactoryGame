/// <summary>
/// Data structure for a single tile of world data. Includes a tile's type, resource type, and resource count (if any).
/// </summary>
/// <remarks>
/// The tile data itself does not store its own X/Y positioning. This should be inferred from its location within a <see cref="TilemapChunk"/>.
/// </remarks>
public struct TileData
{
    /// <summary>
    /// The type of ground tile. For example, is this grass, sand, water, concrete, etc.
    /// </summary>
    // Exact tile type mappings TBD
    public int TileType
    {
        readonly get => tileInt >> 28;
        set => ClearAndSet(value, 0b1111, 28);
    }

    /// <summary>
    /// The resource type on this tile. A <see cref="ResourceType"/> of 0 means there are no resources on this tile.
    /// </summary>
    // Exact resource type mappings TBD
    public int ResourceType
    {
        readonly get => (tileInt >> 24) & 0b1111;
        set => ClearAndSet(value, 0b1111, 24);
    }

    /// <summary>
    /// The amount of resources on this tile. A <see cref="ResourceCount"/> of 0 means that this resource has been depleted.
    /// </summary>
    // 24 bits means the maximum resource capacity on a single tile is ~16 million resources, however our resource patches will be far bigger than one tile.
    public int ResourceCount
    {
        readonly get => tileInt & 0b11111111_11111111_11111111;
        set => ClearAndSet(value, 0b11111111_11111111_11111111, 0);
    }

    // Bit layout:
    // TTTTRRRRCCCCCCCCCCCCCCCCCCCCCCCC
    // T: 4 bits for a tile type [0, 15]
    // R: 4 bits for a resource type (assuming 0 = no resource) [0, 15]
    // C: 24 bits for resource capacity [0, ~16 million]
    // This is not ideal if we wanted to ever have more than 16 tile types or resource types, but for this game proof of concept, I am fine.
    private int tileInt;

    private void ClearAndSet(int value, int bitSize, int bitOffset)
    {
        var mask = bitSize << bitOffset;
        var cleared = tileInt & ~mask;
        tileInt = cleared | (value << bitOffset);
    }
}
