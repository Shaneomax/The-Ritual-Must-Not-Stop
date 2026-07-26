using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Controls the interaction prompt UI element positioned on the screen (e.g. bottom-left corner).
/// Displays prompt text such as "Press E to interact" when the player aims their crosshair
/// at an interactable object within range.
///
/// Setup in Unity Editor:
/// 1. Create a Panel or Text object under your main Canvas anchored to the Bottom-Left.
/// 2. Attach this component to your Canvas or UI Panel object.
/// 3. Assign the PlayerInteractor, Prompt Panel GameObject, and Text component (TMPro or UI.Text).
/// </summary>
public class InteractionUIController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the PlayerInteractor on PlayerCapsule. Auto-found if left unassigned.")]
    [SerializeField] private PlayerInteractor _playerInteractor;

    [Tooltip("The parent GameObject / Panel containing the prompt text widget.")]
    [SerializeField] private GameObject _promptPanel;

    [Tooltip("TextMeshPro text element used for displaying interaction prompt text.")]
    [SerializeField] private TMP_Text _promptText;

    [Tooltip("Legacy UnityEngine.UI.Text fallback component if TextMeshPro is not used.")]
    [SerializeField] private Text _legacyPromptText;

    [Header("Fade Settings (Optional)")]
    [Tooltip("CanvasGroup attached to _promptPanel for smooth fade-in/out transitions. Auto-fetched if present.")]
    [SerializeField] private CanvasGroup _canvasGroup;

    [Tooltip("Enable smooth alpha fading for the interaction prompt UI.")]
    [SerializeField] private bool _useFade = true;

    [Tooltip("Speed at which the UI fades in and out when focusing/unfocusing interactables.")]
    [SerializeField] private float _fadeSpeed = 12f;

    private float _targetAlpha = 0f;

    private void Start()
    {
        // Auto-find PlayerInteractor if not assigned in Inspector
        if (_playerInteractor == null)
        {
            _playerInteractor = FindAnyObjectByType<PlayerInteractor>();
            if (_playerInteractor == null)
            {
                Debug.LogWarning("[InteractionUIController] PlayerInteractor reference is missing and could not be found in the scene.");
            }
        }

        // Auto-get CanvasGroup if prompt panel is assigned
        if (_canvasGroup == null && _promptPanel != null)
        {
            _canvasGroup = _promptPanel.GetComponent<CanvasGroup>();
        }

        // Start hidden
        SetPromptVisible(false, true);
    }

    private void Update()
    {
        if (_playerInteractor == null) return;

        IInteractable interactable = _playerInteractor.CurrentInteractable;
        string description = interactable != null ? interactable.GetDescription() : null;

        bool shouldShow = !string.IsNullOrEmpty(description);

        if (shouldShow)
        {
            UpdatePromptText(description);
        }

        SetPromptVisible(shouldShow);

        // Handle CanvasGroup fading if enabled
        if (_useFade && _canvasGroup != null && _promptPanel != null && _promptPanel.activeSelf)
        {
            _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, _targetAlpha, _fadeSpeed * Time.deltaTime);

            if (_targetAlpha == 0f && Mathf.Approximately(_canvasGroup.alpha, 0f))
            {
                _promptPanel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Updates the text element (TMPro or legacy UI Text) with the given prompt string.
    /// </summary>
    public void UpdatePromptText(string text)
    {
        if (_promptText != null)
        {
            _promptText.text = text;
        }

        if (_legacyPromptText != null)
        {
            _legacyPromptText.text = text;
        }
    }

    /// <summary>
    /// Show or hide the prompt UI panel.
    /// </summary>
    private void SetPromptVisible(bool visible, bool instant = false)
    {
        if (_promptPanel == null) return;

        _targetAlpha = visible ? 1f : 0f;

        if (!_useFade || _canvasGroup == null || instant)
        {
            _promptPanel.SetActive(visible);
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = _targetAlpha;
            }
        }
        else if (visible && !_promptPanel.activeSelf)
        {
            _promptPanel.SetActive(true);
            if (_canvasGroup != null && instant)
            {
                _canvasGroup.alpha = 1f;
            }
        }
    }
}
