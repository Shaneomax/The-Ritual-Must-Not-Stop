using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks items the player is carrying. Attach to PlayerCapsule alongside PlayerContext.
/// Use HasItem() to gate interactions — replaces all child-name searches.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    private readonly List<ItemType> _items = new List<ItemType>();

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Returns true if the player is carrying at least one item of the given type.</summary>
    public bool HasItem(ItemType type) => _items.Contains(type);
    
    /// <summary>Returns the exact number of items of this type the player has.</summary>
    public int GetItemCount(ItemType type)
    {
        int count = 0;
        foreach (var item in _items)
        {
            if (item == type) count++;
        }
        return count;
    }

    /// <summary>Adds an item to the inventory. Can hold multiple of the same type.</summary>
    public void AddItem(ItemType type)
    {
        _items.Add(type);
        Debug.Log($"[Inventory] Added: {type} (Total: {GetItemCount(type)})");
    }

    /// <summary>Removes one instance of an item from the inventory. Returns true if successful.</summary>
    public bool RemoveItem(ItemType type)
    {
        bool removed = _items.Remove(type);
        if (removed) Debug.Log($"[Inventory] Removed: {type}");
        return removed;
    }

    /// <summary>Read-only view of all currently held items.</summary>
    public IReadOnlyCollection<ItemType> GetItems() => _items.AsReadOnly();

    /// <summary>Removes all items from the inventory.</summary>
    public void ClearInventory()
    {
        _items.Clear();
        Debug.Log("[Inventory] Cleared all items.");
    }
}
