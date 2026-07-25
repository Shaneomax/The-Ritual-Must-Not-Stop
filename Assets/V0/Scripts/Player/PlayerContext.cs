using UnityEngine;
using StarterAssets;

/// <summary>
/// Central player reference hub. Attach to PlayerCapsule.
/// Drag this into any script that needs player components — no runtime searches needed.
/// </summary>
public class PlayerContext : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private FirstPersonController _fpsController;
    [SerializeField] private StarterAssetsInputs _input;

    [Header("Camera")]
    [Tooltip("Drag the MainCamera here. Used by pickup animations for eye-level targeting.")]
    [SerializeField] private Camera _mainCamera;

    [Header("Systems")]
    [SerializeField] private PlayerInventory _inventory;

    // ── Public Accessors ─────────────────────────────────────────────────────
    public CharacterController   CharacterController => _characterController;
    public FirstPersonController FPSController       => _fpsController;
    public StarterAssetsInputs   Input               => _input;
    public PlayerInventory       Inventory           => _inventory;
    public Camera                MainCamera          => _mainCamera;

    // ── Player Freeze / Unfreeze ─────────────────────────────────────────────
    /// <summary>Disables movement and physics so a cutscene or animation can take over.</summary>
    public void FreezePlayer()
    {
        if (_fpsController != null)      _fpsController.enabled      = false;
        if (_characterController != null) _characterController.enabled = false;
    }

    /// <summary>Re-enables movement and physics after a cutscene or animation.</summary>
    public void UnfreezePlayer()
    {
        if (_fpsController != null)      _fpsController.enabled      = true;
        if (_characterController != null) _characterController.enabled = true;
    }

    // ── Auto-fill in Editor ───────────────────────────────────────────────────
    private void OnValidate()
    {
        if (_characterController == null) _characterController = GetComponent<CharacterController>();
        if (_fpsController == null)       _fpsController       = GetComponent<FirstPersonController>();
        if (_input == null)               _input               = GetComponent<StarterAssetsInputs>();
        if (_inventory == null)           _inventory           = GetComponent<PlayerInventory>();
        if (_mainCamera == null)          _mainCamera          = Camera.main;
    }
}
