using UnityEngine;

/// <summary>
/// Interactable candle. Lights with a Lighter, otherwise picks up.
/// Requires the player to have ItemType.Lighter in their PlayerInventory.
///
/// Inspector setup:
///   - Candle Flame   → drag the child ParticleSystem here
///   - Player Context → drag PlayerCapsule here
///   - Prompt Text    → e.g. "Press E to Light (with Lighter) or Pick Up"
/// </summary>
public class InteractableCandle : InteractableBase
{
    [Header("Candle Settings")]
    [SerializeField] private ParticleSystem _candleFlame;
    [SerializeField] private bool _isLit = false;

    // ── IInteractable ─────────────────────────────────────────────────────────

    public override string GetDescription()
    {
        return _isLit ? "The candle is burning." : _promptText;
    }

    public override void Interact()
    {
        if (_isLit) return;

        bool hasLighter = _playerContext != null && _playerContext.Inventory.HasItem(ItemType.Lighter);

        if (hasLighter)
            LightCandle();
        else
            PickUpCandle();
    }

    // ── Private Helpers ───────────────────────────────────────────────────────

    private void LightCandle()
    {
        _isLit = true;

        if (_candleFlame != null)
        {
            ParticleSystem.EmissionModule emission = _candleFlame.emission;
            emission.enabled = true;
            _candleFlame.Play();
        }

        Debug.Log("Candle lit!");
    }

    private void PickUpCandle()
    {
        Debug.Log("You picked up the unlit candle!");
        // Add to inventory when an inventory system is in place:
        // _playerContext?.Inventory.AddItem(ItemType.Candle);
        Destroy(gameObject);
    }
}