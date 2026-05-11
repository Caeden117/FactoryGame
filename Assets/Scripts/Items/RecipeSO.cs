using System;
using UnityEngine;

/// <summary>
/// A recipe data object. This should be referenced for crafting operations and should not be used for inventory management.
/// </summary>
[CreateAssetMenu(fileName = "RecipeSO", menuName = "Factory Game/RecipeSO")]
public class RecipeSO : ScriptableObject
{
    [Serializable]
    public class Ingredient
    {
        public ItemSO Item;
        public int Amount;
    }

    [Serializable]
    public class Output
    {
        public ItemSO Item;
        public int Amount;
    }

    [NonSerialized] public int Id;

    public Ingredient[] Ingredients;
    public Output[] Outputs;

    /// <summary>
    /// Base crafting time for this recipe in seconds. This does not consider a machine's crafting speed multiplier.
    /// </summary>
    public float CraftingTime = 1f;
}
