using UnityEngine;
using StarterAssets;

/// <summary>
/// Sits on PlayerCapsule. Fires a raycast from the camera each frame.
/// When the player looks at an IInteractable and presses E, Interact() is called.
///
/// Inspector setup:
///   - Main Camera         → drag MainCamera here (auto-finds Camera.main if left empty)
///   - Interaction Distance → max raycast range in metres
///   - Interactable Layer  → set this to your Interactable layer mask
///
/// NOTE: GetComponentInParent is used so that child colliders (e.g. Part1, Part2 of a
/// door/closet mesh) correctly resolve the IInteractable on their parent GameObject.
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _interactionDistance = 3f;
    [SerializeField] private LayerMask _interactableLayer;

    private StarterAssetsInputs _input;

    /// <summary>The interactable currently in the player's crosshair, or null.</summary>
    public IInteractable CurrentInteractable { get; private set; }

    private void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();

        if (_mainCamera == null)
            _mainCamera = Camera.main;

        if (_input == null)
            Debug.LogError("[PlayerInteractor] StarterAssetsInputs not found on this GameObject. " +
                           "Make sure PlayerInteractor is on the same GameObject as StarterAssetsInputs (PlayerCapsule).");

        if (_mainCamera == null)
            Debug.LogError("[PlayerInteractor] No camera found. Drag MainCamera into the Inspector slot.");
    }

    private void Update()
    {
        if (_input == null || _mainCamera == null) return;

        ScanForInteractable();

        if (_input.interact)
        {
            CurrentInteractable?.Interact();
            _input.interact = false;
        }
    }

    // ── Private ───────────────────────────────────────────────────────────────

    private void ScanForInteractable()
    {
        Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _interactionDistance, _interactableLayer))
        {
            // Use GetComponentInParent so raycasts hitting a child mesh/collider
            // (e.g. Part1 of a closet) still find the IInteractable on the root object.
            CurrentInteractable = hit.collider.GetComponentInParent<IInteractable>();
        }
        else
        {
            CurrentInteractable = null;
        }
    }
}