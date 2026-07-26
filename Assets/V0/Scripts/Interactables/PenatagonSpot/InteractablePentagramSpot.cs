using System.Collections;
using UnityEngine;

/// <summary>
/// Interactable pentagram spot. Player must be inside the trigger zone and
/// have ItemType.Chalk in inventory to draw. Plays a cinematic look-down sequence.
///
/// Inspector setup:
///   - Pentagram Object → the mesh/decal to activate when drawn
///   - Look Down Camera → the Cinemachine virtual camera to activate during the cutscene
///   - Player Context   → drag PlayerCapsule here
///   - Prompt Text      → e.g. "Press E to Draw Pentagram (Requires Chalk)"
/// </summary>
public class InteractablePentagramSpot : InteractableBase
{
    [Header("Pentagram Target")]
    [SerializeField] private GameObject _pentagramObject;
    [Tooltip("How long to pause after drawing before the camera sweeps back up.")]
    [SerializeField] private float _drawDelay = 1.0f;

    [Header("Cinemachine Settings")]
    [Tooltip("Drag the PentagramCam Cinemachine virtual camera here.")]
    [SerializeField] private GameObject _lookDownCamera;
    [Tooltip("Should match the Cinemachine Brain default blend time.")]
    [SerializeField] private float _cameraBlendTime = 1.5f;

    private bool _isDrawn        = false;
    private bool _isPlayerInside = false;
    private bool _isAnimating    = false;

    private void Start()
    {
        if (_pentagramObject != null)
            _pentagramObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) _isPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) _isPlayerInside = false;
    }

    // ── IInteractable ─────────────────────────────────────────────────────────

    public override string GetDescription()
    {
        if (_isDrawn)       return "The pentagram is drawn.";
        if (_isAnimating)   return "";
        if (!_isPlayerInside) return "Get closer to draw the Pentagram.";
        return _promptText;
    }

    public override void Interact()
    {
        if (_isDrawn || !_isPlayerInside || _isAnimating) return;

        if (_playerContext != null && _playerContext.Inventory.HasItem(ItemType.Chalk))
        {
            StartCoroutine(DrawPentagramSequence());
        }
        else
        {
            Debug.Log("You need chalk in your inventory to draw this!");
        }
    }

    // ── Coroutine ─────────────────────────────────────────────────────────────

    private IEnumerator DrawPentagramSequence()
    {
        _isAnimating = true;

        _playerContext.FreezePlayer();
        if (_lookDownCamera != null) _lookDownCamera.SetActive(true);

        yield return new WaitForSeconds(_cameraBlendTime);

        if (_pentagramObject != null) _pentagramObject.SetActive(true);

        yield return new WaitForSeconds(_drawDelay);

        if (_lookDownCamera != null) _lookDownCamera.SetActive(false);

        yield return new WaitForSeconds(_cameraBlendTime);

        _playerContext.UnfreezePlayer();

        _isDrawn     = true;
        _isAnimating = false;
        Debug.Log("Pentagram drawn successfully!");
    }
}