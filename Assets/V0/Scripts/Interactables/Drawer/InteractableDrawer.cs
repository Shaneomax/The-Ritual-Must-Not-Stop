using UnityEngine;

/// <summary>
/// Interactable drawer. Supports simple open/close animation toggling.
///
/// Inspector setup:
///   - Player Context → drag PlayerCapsule here
///   - Prompt Text    → e.g. "Press E to Open Drawer"
/// </summary>
public class InteractableDrawer : InteractableBase
{
    [Header("Drawer Settings")]
    [SerializeField] private bool _isOpen = false;
    
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // ── IInteractable ─────────────────────────────────────────────────────────

    public override string GetDescription()
    {
        if (_isOpen) return "Press E to Close Drawer";
        return _promptText; // Uses your default prompt like "Press E to Open Drawer"
    }

    public override void Interact()
    {
        ToggleDrawer();
    }

    // ── Private Helpers ───────────────────────────────────────────────────────

    private void ToggleDrawer()
    {
        _isOpen = !_isOpen;
        Debug.Log(_isOpen ? "Drawer Opened!" : "Drawer Closed!");
        
        // This triggers the animation by toggling a boolean parameter called "IsOpen" in your Animator
        if (_animator != null)
        {
            _animator.SetBool("IsOpen", _isOpen);
        }
        else
        {
            Debug.LogWarning("No Animator found on the Drawer!");
        }
    }
}
