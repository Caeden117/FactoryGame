using UnityEngine;

/// <summary>
/// Library that holds references to all items and recipes in the game.
/// In scripts, items and recipes are referenced by their integer ID, which corresponds to their index in the respective array in this library.
/// This library should be used as a reference for pulling specific details about items and recipes, but should not be used for inventory management.
/// Inventory management should still be done by storing item IDs along with their stack count.
/// </summary>
[CreateAssetMenu(fileName = "Item Library", menuName = "Factory Game/ItemLibrarySO")]
public class ItemLibrarySO : ScriptableObject
{
    /// <summary>
    /// All registered items in the game
    /// </summary>
    public ItemSO[] Items;

    /// <summary>
    /// All registered recipes in the game
    /// </summary>
    public RecipeSO[] Recipes;

    // Assign IDs to items and recipes based on their index in the array. This allows for easy referencing by ID without needing to serialize the ID field.
    private void OnValidate()
    {
        for (var i = 0; i < Items.Length; i++)
        {
            if (Items[i] != null)
            {
                Items[i].Id = i;
            }
        }

        for (var i = 0; i < Recipes.Length; i++)
        {
            if (Items[i] != null)
            {
                Recipes[i].Id = i;
            }
        }
    }
}
