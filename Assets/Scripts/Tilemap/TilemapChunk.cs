using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapChunk : MonoBehaviour
{
    private Dictionary<Vector3Int, int> resourceQuantities = new Dictionary<Vector3Int, int>();

    [SerializeField] private Tilemap resourceTilemap;
    [SerializeField] private Tilemap groundTilemap;

    public ItemSO GetResourceAt(Vector3 worldPosition)
    {
        var cellPosition = resourceTilemap.WorldToCell(worldPosition);
        var tile = resourceTilemap.GetTile(cellPosition);

        return tile is ResourceTile resourceTile ? resourceTile.ResourceItem : null;
    }

    public bool DrillResourceAt(Vector3 worldPosition, int drillAmount = 1)
    {
        var cellPosition = resourceTilemap.WorldToCell(worldPosition);
        var tile = resourceTilemap.GetTile(cellPosition);

        if (tile is ResourceTile resourceTile && resourceQuantities.TryGetValue(cellPosition, out var quantity) && quantity > 0)
        {
            var currentQuantity = resourceQuantities.ContainsKey(cellPosition) ? resourceQuantities[cellPosition] : resourceTile.ResourceQuantity;
            var newQuantity = Mathf.Max(currentQuantity - drillAmount, 0);
            SetResourceQuantity(cellPosition, resourceTile, newQuantity);
            return true;
        }

        return false;
    }

    public void SetResourceQuantity(Vector3Int position, ResourceTile resourceTile, int quantity)
    {
        resourceQuantities[position] = quantity;
        if (quantity <= 0)
        {
            resourceQuantities.Remove(position);
            resourceTilemap.SetTile(position, null);
        }
        else
        {
            resourceTilemap.SetTile(position, resourceTile);
        }
    }

    public void SetGroundTile(Vector3Int position, TileBase groundTile)
    {
        groundTilemap.SetTile(position, groundTile);
    }

    // THIS IS SLOW BUT ONE DAY WE WILL HAVE PROCEDURAL GENERATION
    private void Discover()
    {
        var tileArray = resourceTilemap.GetTiles(resourceTilemap.cellBounds, Allocator.Temp);
        for (var x = 0; x < resourceTilemap.size.x; x++)
        {
            for (var y = 0; y < resourceTilemap.size.y; y++)
            {
                var tile = tileArray[x + y * resourceTilemap.size.x];
                if (tile is ResourceTile resourceTile)
                {
                    var cellPosition = new Vector3Int(x, y, 0) + resourceTilemap.cellBounds.min;
                    var quantity = resourceQuantities.ContainsKey(cellPosition) ? resourceQuantities[cellPosition] : resourceTile.ResourceQuantity;
                    Debug.Log($"Discovered resource at {cellPosition} with quantity {quantity}");
                }
            }
        }
    }

    private void Start() => Discover();
}
