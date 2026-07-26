using UnityEngine;
using StarterAssets;

/// <summary>
/// Toggles the player's flashlight on/off via the Flashlight input action.
/// Attach to the FlashlightController GameObject (child of PlayerCameraRoot or PlayerCapsule).
///
/// Inspector setup:
///   - Flashlight Light → drag your Spot Light component here
///   - Input            → drag the StarterAssetsInputs component from PlayerCapsule here
/// </summary>
public class FlashlightController : MonoBehaviour
{
    [Header("Flashlight Settings")]
    [SerializeField] private Light _flashlightLight;
    [SerializeField] private bool _isLit = false;

    [Header("References")]
    [SerializeField] private StarterAssetsInputs _input;

    private void Start()
    {
        if (_flashlightLight != null)
            _flashlightLight.enabled = _isLit;
    }

    private void Update()
    {
        if (_input == null || !_input.flashlight) return;

        ToggleFlashlight();
        _input.flashlight = false; // Consume the input so it doesn't repeat every frame
    }

    private void ToggleFlashlight()
    {
        _isLit = !_isLit;
        if (_flashlightLight != null) _flashlightLight.enabled = _isLit;
        Debug.Log(_isLit ? "Flashlight ON" : "Flashlight OFF");
    }
}