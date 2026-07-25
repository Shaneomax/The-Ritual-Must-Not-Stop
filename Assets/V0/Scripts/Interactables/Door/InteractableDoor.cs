using UnityEngine;

/// <summary>
/// Interactable door. Supports open/close and locked state requiring a Key.
/// Requires the player to have ItemType.Key in their PlayerInventory to unlock.
///
/// Inspector setup:
///   - Player Context → drag PlayerCapsule here
///   - Prompt Text    → e.g. "Press E to Open Door"
///   - Tag the GameObject as "Locked" in the Inspector to make it require a key
/// </summary>
public class InteractableDoor : InteractableBase
{
    [Header("Door Settings")]
    [SerializeField] private bool _isOpen = false;
    
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // ── IInteractable ─────────────────────────────────────────────────────────

    public override string GetDescription()
    {
        if (_isOpen) return "Press E to Close Door";
        if (gameObject.CompareTag("Locked")) return "Press E to Unlock Door (Requires Key)";
        return _promptText;
    }

    public override void Interact()
    {
        // Locked door — check inventory for key first
        if (!_isOpen && gameObject.CompareTag("Locked"))
        {
            if (_playerContext != null && _playerContext.Inventory.HasItem(ItemType.Key))
            {
                UnlockDoor();
            }
            else
            {
                Debug.Log("The door is locked. You need a key!");
                return;
            }
        }

        ToggleDoor();
    }

    // ── Private Helpers ───────────────────────────────────────────────────────

    private void ToggleDoor()
    {
        _isOpen = !_isOpen;
        Debug.Log(_isOpen ? "Door Opened!" : "Door Closed!");
        _animator?.SetBool("IsOpen", _isOpen);
    }

    private void UnlockDoor()
    {
        Debug.Log("Door unlocked with the key!");
        // Remove the Locked tag so the key is not required again for this door
        gameObject.tag = "Untagged";
    }
}