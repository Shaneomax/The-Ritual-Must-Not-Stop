using UnityEngine;

/// <summary>
/// Abstract base class for all interactable objects.
/// Extend this instead of implementing IInteractable directly.
///
/// In the Inspector, set:
///   - Prompt Text    → the string shown to the player when in range
///   - Player Context → drag PlayerCapsule here (gives access to inventory, freeze, input)
/// </summary>
public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [Tooltip("Text shown in the interaction UI when the player looks at this object.")]
    [SerializeField] protected string _promptText = "Press E to Interact";

    [Tooltip("Drag the PlayerCapsule GameObject here.")]
    [SerializeField] protected PlayerContext _playerContext;

    // ── IInteractable ─────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the prompt string. Override in subclasses to return dynamic descriptions
    /// (e.g. "The candle is burning" vs "Press E to light candle").
    /// </summary>
    public virtual string GetDescription() => _promptText;

    /// <summary>Called by PlayerInteractor when the player presses the interact key.</summary>
    public abstract void Interact();
}
