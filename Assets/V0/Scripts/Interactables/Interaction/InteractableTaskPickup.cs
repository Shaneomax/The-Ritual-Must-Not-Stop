using UnityEngine;

/// <summary>
/// Task-completing item pickup. Adds the item to inventory AND marks a GameTask complete.
///
/// Inspector setup:
///   - Item Data        → drag the ItemData ScriptableObject here
///   - Task To Complete → drag the GameTask ScriptableObject here
///   - Player Context   → drag PlayerCapsule here
///   - Prompt Text      → defaults to "Press E to pick up [name]"
///
/// Optional animation:
///   Add a <see cref="PickupAnimationHandler"/> component to this same GameObject
///   to get a smooth DOTween rise + fly-in animation before the item is collected.
///   If no handler is found, the item is collected instantly (useful for testing).
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

        PickupAnimationHandler anim = GetComponent<PickupAnimationHandler>();

        if (anim != null && _playerContext != null)
        {
            // Guard: don't retrigger if animation is already playing
            if (anim.IsPlaying) return;

            anim.Play(_playerContext, OnCollected);
        }
        else
        {
            // No animation component — collect immediately
            OnCollected();
        }
    }

    // ── Private ───────────────────────────────────────────────────────────────

    /// <summary>Fires when the item is ready to be collected (after animation or instantly).</summary>
    private void OnCollected()
    {
        Debug.Log($"You picked up {_itemData.itemName}!");
        _playerContext?.Inventory.AddItem(_itemData.type);

        if (_taskToComplete != null)
        {
            _taskToComplete.isCompleted = true;
            Debug.Log($"<color=green>SUCCESS:</color> Task Completed: {_taskToComplete.taskName}");
        }
        else
        {
            Debug.LogWarning("<color=red>WARNING:</color> You picked up the item, but 'Task To Complete' is completely empty (null) on this object!");
        }

        Destroy(gameObject);
    }
}