using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ResourceTile", menuName = "Factory Game/ResourceTile")]
public class ResourceTile : TileBase
{
    public Sprite ResourceSprite;
    public ItemSO ResourceItem;
    public int ResourceQuantity = 100;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = ResourceSprite;
        tileData.color = Color.white;
        tileData.transform = Matrix4x4.identity;
        tileData.gameObject = null;
        tileData.flags = TileFlags.LockAll;
        tileData.colliderType = Tile.ColliderType.None;
    }
}
