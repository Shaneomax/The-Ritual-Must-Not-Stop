using System.Collections;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Interactable closet. Smoothly animates the player inside, lets them hide,
/// then exits on a second interact press.
///
/// Inspector setup:
///   - Hide Point     → Transform marking the player's position inside the closet
///   - Exit Point     → Transform marking where the player lands after exiting
///   - Player Context → drag PlayerCapsule here
///   - Prompt Text    → e.g. "Press E to Hide"
/// </summary>
public class InteractableCloset : InteractableBase
{
    [Header("Closet Settings")]
    [SerializeField] private Transform _hidePoint;
    [SerializeField] private Transform _exitPoint;
    [Tooltip("Seconds to wait for the door open/close animation before moving the player.")]
    [SerializeField] private float _animationDelay = 1.0f;
    [Tooltip("How long the player glides into / out of the closet.")]
    [SerializeField] private float _moveDuration = 0.75f;

    private Animator _animator;
    private bool _isHiding    = false;
    private bool _isAnimating = false;

    // Stored so we can restore the CharacterController after exiting
    private float   _originalHeight;
    private Vector3 _originalCenter;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // While hiding, a second E press triggers exit
        if (_isHiding && !_isAnimating && _playerContext != null && _playerContext.Input.interact)
        {
            _playerContext.Input.interact = false;
            StartCoroutine(ExitClosetSequence());
        }
    }

    // ── IInteractable ─────────────────────────────────────────────────────────

    public override string GetDescription() => _isHiding ? "" : _promptText;

    public override void Interact()
    {
        if (!_isHiding && !_isAnimating)
            StartCoroutine(EnterClosetSequence());
    }

    // ── Coroutines ────────────────────────────────────────────────────────────

    private IEnumerator EnterClosetSequence()
    {
        _isAnimating = true;

        // Open the door
        _animator.SetBool("IsOpen", true);
        yield return new WaitForSeconds(_animationDelay);

        // Cache original CC dimensions before crouch
        _originalHeight = _playerContext.CharacterController.height;
        _originalCenter = _playerContext.CharacterController.center;

        _playerContext.FreezePlayer();

        // Glide player to hide point
        _playerContext.transform.DOMove(_hidePoint.position, _moveDuration).SetEase(Ease.InOutSine);
        _playerContext.transform.DORotate(_hidePoint.rotation.eulerAngles, _moveDuration).SetEase(Ease.InOutSine);

        // Crouch the CharacterController to fit the closet
        DOTween.To(() => _playerContext.CharacterController.height,
                   x  => _playerContext.CharacterController.height = x,
                   1.0f, _moveDuration);
        DOTween.To(() => _playerContext.CharacterController.center,
                   x  => _playerContext.CharacterController.center = x,
                   new Vector3(0f, 0.5f, 0f), _moveDuration);

        yield return new WaitForSeconds(_moveDuration);

        // Close the door
        _animator.SetBool("IsOpen", false);
        yield return new WaitForSeconds(_animationDelay);

        _isHiding    = true;
        _isAnimating = false;
    }

    private IEnumerator ExitClosetSequence()
    {
        _isAnimating = true;

        // Open the door
        _animator.SetBool("IsOpen", true);
        yield return new WaitForSeconds(_animationDelay);

        // Glide player back to exit point
        _playerContext.transform.DOMove(_exitPoint.position, _moveDuration).SetEase(Ease.InOutSine);
        _playerContext.transform.DORotate(_exitPoint.rotation.eulerAngles, _moveDuration).SetEase(Ease.InOutSine);

        // Restore CC to standing dimensions
        DOTween.To(() => _playerContext.CharacterController.height,
                   x  => _playerContext.CharacterController.height = x,
                   _originalHeight, _moveDuration);
        DOTween.To(() => _playerContext.CharacterController.center,
                   x  => _playerContext.CharacterController.center = x,
                   _originalCenter, _moveDuration);

        yield return new WaitForSeconds(_moveDuration);

        _playerContext.UnfreezePlayer();

        // Close the door behind them
        _animator.SetBool("IsOpen", false);
        yield return new WaitForSeconds(_animationDelay);

        _isHiding    = false;
        _isAnimating = false;
    }
}