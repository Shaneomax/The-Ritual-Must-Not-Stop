using UnityEngine;

/// <summary>
/// Simple item pickup. Adds the item to the player's inventory on interact.
///
/// Inspector setup:
///   - Item Data      → drag the ItemData ScriptableObject here
///   - Player Context → drag PlayerCapsule here
///   - Prompt Text    → override defaults to "Press E to pick up [name]"
/// </summary>
public class InteractablePickup : InteractableBase
{
    [Header("Item Settings")]
    [SerializeField] private ItemData _itemData;

    // ── IInteractable ─────────────────────────────────────────────────────────

    public override string GetDescription()
    {
        return _itemData != null ? $"Press E to pick up {_itemData.itemName}" : _promptText;
    }

    public override void Interact()
    {
        if (_itemData == null) return;

        Debug.Log($"You picked up {_itemData.itemName}!");
        _playerContext?.Inventory.AddItem(_itemData.type);
        Destroy(gameObject);
    }
}