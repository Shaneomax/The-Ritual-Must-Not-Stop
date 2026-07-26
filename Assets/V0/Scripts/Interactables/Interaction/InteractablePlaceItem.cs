using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An interactable that requires the player to have a specific number of an item in their inventory.
/// Great for placing candles on a pentagram, putting keys in a lock, etc.
/// </summary>
public class InteractablePlaceItem : InteractableBase
{
    [Header("Requirement Settings")]
    [Tooltip("The type of item the player must have in their inventory.")]
    [SerializeField] private ItemType _requiredItem;
    
    [Tooltip("How many of this item are required?")]
    [SerializeField] private int _requiredAmount = 6;

    [Tooltip("Should the items be removed from the player's inventory when placed?")]
    [SerializeField] private bool _consumeItems = true;

    [Header("Placement Effects")]
    [Tooltip("Objects to turn on when the items are successfully placed (e.g., the candles on the floor).")]
    [SerializeField] private GameObject[] _objectsToActivate;

    [Tooltip("Fires when the items are successfully placed.")]
    public UnityEvent OnSuccessfullyPlaced;
    
    [Tooltip("Fires if the player tries to interact but doesn't have enough items.")]
    public UnityEvent OnMissingItems;

    // ── IInteractable ─────────────────────────────────────────────────────────

    public override string GetDescription()
    {
        if (_playerContext == null) return _promptText;

        int currentCount = _playerContext.Inventory.GetItemCount(_requiredItem);

        if (currentCount >= _requiredAmount)
        {
            return _promptText; // e.g. "Press E to Place Candles"
        }
        else
        {
            return $"Need {_requiredAmount} {_requiredItem}s. (You have {currentCount})";
        }
    }

    public override void Interact()
    {
        if (_playerContext == null) return;

        int currentCount = _playerContext.Inventory.GetItemCount(_requiredItem);

        if (currentCount >= _requiredAmount)
        {
            // 1. Optionally remove the items from the player's inventory
            if (_consumeItems)
            {
                for (int i = 0; i < _requiredAmount; i++)
                {
                    _playerContext.Inventory.RemoveItem(_requiredItem);
                }
            }

            // 2. Activate all target objects (like the candles on the floor)
            if (_objectsToActivate != null)
            {
                foreach (GameObject obj in _objectsToActivate)
                {
                    if (obj != null) obj.SetActive(true);
                }
            }

            // 3. Fire success event
            OnSuccessfullyPlaced?.Invoke();
            Debug.Log($"<color=green>SUCCESS:</color> Placed {_requiredAmount} {_requiredItem}(s)!");

            // 4. Disable this trigger so it can't be used again
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
            this.enabled = false;
        }
        else
        {
            // Not enough items!
            OnMissingItems?.Invoke();
            Debug.LogWarning($"<color=orange>FAILED:</color> Not enough {_requiredItem}s to place. Need {_requiredAmount}, have {currentCount}.");
        }
    }
}
