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
    
    [Tooltip("How many of this item does the player need to complete the task? (e.g. 5 for candles)")]
    [SerializeField] private int _requiredAmountToCompleteTask = 1;

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
        // 1. Add item to inventory (which handles the count internally now)
        _playerContext?.Inventory.AddItem(_itemData.type);

        // 2. Check if we have enough to complete the task
        if (_taskToComplete != null)
        {
            int currentCount = _playerContext.Inventory.GetItemCount(_itemData.type);
            
            if (currentCount >= _requiredAmountToCompleteTask)
            {
                _taskToComplete.isCompleted = true;
                Debug.Log($"<color=green>SUCCESS:</color> Task Completed: {_taskToComplete.taskName}");
            }
            else
            {
                Debug.Log($"Task Progress: {currentCount} / {_requiredAmountToCompleteTask} {_itemData.itemName}s collected.");
            }
        }
        else
        {
            Debug.LogWarning("<color=red>WARNING:</color> You picked up the item, but 'Task To Complete' is completely empty (null) on this object!");
        }

        Destroy(gameObject);
    }
}