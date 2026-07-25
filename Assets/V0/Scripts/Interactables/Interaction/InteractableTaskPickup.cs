using UnityEngine;

/// <summary>
/// Task-completing item pickup. Adds the item to inventory AND marks a GameTask complete.
///
/// Inspector setup:
///   - Item Data        → drag the ItemData ScriptableObject here
///   - Task To Complete → drag the GameTask ScriptableObject here
///   - Player Context   → drag PlayerCapsule here
///   - Prompt Text      → override defaults to "Press E to pick up [name]"
/// </summary>
public class InteractableTaskPickup : InteractableBase
{
    [Header("Task Settings")]
    [Tooltip("The ScriptableObject task this pickup completes.")]
    [SerializeField] private GameTask _taskToComplete;

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

        if (_taskToComplete != null)
        {
            _taskToComplete.isCompleted = true;
            Debug.Log($"Task Completed: {_taskToComplete.taskName}");
        }

        Destroy(gameObject);
    }
}