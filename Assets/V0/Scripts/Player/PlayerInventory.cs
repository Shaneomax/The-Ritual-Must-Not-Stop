using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks items the player is carrying. Attach to PlayerCapsule alongside PlayerContext.
/// Use HasItem() to gate interactions — replaces all child-name searches.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    private readonly HashSet<ItemType> _items = new HashSet<ItemType>();

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Returns true if the player is carrying an item of the given type.</summary>
    public bool HasItem(ItemType type) => _items.Contains(type);

    /// <summary>Adds an item to the inventory. Duplicate types are ignored (Set semantics).</summary>
    public void AddItem(ItemType type)
    {
        _items.Add(type);
        Debug.Log($"[Inventory] Added: {type}");
    }

    /// <summary>Removes an item from the inventory. Returns true if the item was present.</summary>
    public bool RemoveItem(ItemType type)
    {
        bool removed = _items.Remove(type);
        if (removed) Debug.Log($"[Inventory] Removed: {type}");
        return removed;
    }

    /// <summary>Read-only view of all currently held items.</summary>
    public IReadOnlyCollection<ItemType> GetItems() => _items;
}
