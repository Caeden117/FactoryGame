using System;
using UnityEngine;

/// <summary>
/// The visual/data representation of an item. This should not be used for inventory management, but rather to pull details about an item being worked on.
/// </summary>
[CreateAssetMenu(fileName = "ItemSO", menuName = "Factory Game/ItemSO")]
public class ItemSO : ScriptableObject
{
    /// <summary>
    /// Internal ID used for referencing this item.
    /// </summary>
    [NonSerialized] public int Id;

    /// <summary>
    /// The name of the item to be displayed in the UI.
    /// </summary>
    public string DisplayName;

    /// <summary>
    /// The icon to be displayed in the UI (or on belts) for this item.
    /// </summary>
    public Sprite Icon;

    /// <summary>
    /// The value of the item.
    /// </summary>
    public int MoneyValue = 1;
}
