using UnityEngine;

/// <summary>
/// Library of supported recipes for a specific object.
/// </summary>
[CreateAssetMenu(fileName = "RecipeListSO", menuName = "Factory Game/RecipeListSO")]
public class RecipeListSO : ScriptableObject
{
    /// <summary>
    /// All supported recipes for this recipe list.
    /// </summary>
    public RecipeSO[] SupportedRecipes;
}
