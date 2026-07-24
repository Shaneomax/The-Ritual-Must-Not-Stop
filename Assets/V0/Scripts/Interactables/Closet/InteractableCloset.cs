using System.Collections;
using UnityEngine;
using StarterAssets; 
using DG.Tweening; // Required for DOTween

public class InteractableCloset : MonoBehaviour, IInteractable
{
    [Header("Closet Settings")]
    public Transform hidePoint;
    public Transform exitPoint;
    public float animationDelay = 1.0f; 
    [Tooltip("How long it takes for the player to smoothly walk into/out of the closet")]
    public float moveDuration = 0.75f; 

    private Animator _animator;
    private bool _isHiding = false;
    private bool _isAnimating = false; 

    // Player References
    private GameObject _player;
    private CharacterController _characterController;
    private FirstPersonController _fpsController;
    private StarterAssetsInputs _playerInput;

    // Original Player Dimensions
    private float _originalHeight;
    private Vector3 _originalCenter;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_isHiding && !_isAnimating && _playerInput != null)
        {
            if (_playerInput.interact)
            {
                _playerInput.interact = false; 
                StartCoroutine(ExitClosetSequence());
            }
        }
    }

    public string GetDescription()
    {
        return _isHiding ? "" : "Press E to Hide"; 
    }

    public void Interact()
    {
        if (!_isHiding && !_isAnimating)
        {
            StartCoroutine(EnterClosetSequence());
        }
    }

    private IEnumerator EnterClosetSequence()
    {
        _isAnimating = true;

        // 1. Find the player and their components
        _player = GameObject.FindGameObjectWithTag("Player");
        _characterController = _player.GetComponent<CharacterController>();
        _fpsController = _player.GetComponent<FirstPersonController>();
        _playerInput = _player.GetComponent<StarterAssetsInputs>();

        // 2. Open the door
        _animator.SetBool("IsOpen", true);

        // 3. Wait for the animation to play
        yield return new WaitForSeconds(animationDelay);

        // 4. Disable player movement scripts so they don't fight DOTween
        _characterController.enabled = false;
        _fpsController.enabled = false;

        // 5. Store original dimensions
        _originalHeight = _characterController.height;
        _originalCenter = _characterController.center;
        
        // 6. Smoothly move and rotate the player into the closet
        _player.transform.DOMove(hidePoint.position, moveDuration).SetEase(Ease.InOutSine);
        _player.transform.DORotate(hidePoint.rotation.eulerAngles, moveDuration).SetEase(Ease.InOutSine);

        // Smoothly adjust the character controller size (Crouching)
        DOTween.To(() => _characterController.height, x => _characterController.height = x, 1.0f, moveDuration);
        DOTween.To(() => _characterController.center, x => _characterController.center = x, new Vector3(0, 1.0f / 2f, 0), moveDuration);

        // Wait for the DOTween movement to completely finish
        yield return new WaitForSeconds(moveDuration);

        // 7. Close the door
        _animator.SetBool("IsOpen", false);

        yield return new WaitForSeconds(animationDelay);

        _isHiding = true;
        _isAnimating = false;
    }

    private IEnumerator ExitClosetSequence()
    {
        _isAnimating = true;

        // 1. Open the door
        _animator.SetBool("IsOpen", true);

        yield return new WaitForSeconds(animationDelay);

        // 2. Smoothly move and rotate the player back outside
        _player.transform.DOMove(exitPoint.position, moveDuration).SetEase(Ease.InOutSine);
        _player.transform.DORotate(exitPoint.rotation.eulerAngles, moveDuration).SetEase(Ease.InOutSine);

        // 3. Smoothly restore the player's original size (Standing up)
        DOTween.To(() => _characterController.height, x => _characterController.height = x, _originalHeight, moveDuration);
        DOTween.To(() => _characterController.center, x => _characterController.center = x, _originalCenter, moveDuration);

        // Wait for the DOTween movement to completely finish
        yield return new WaitForSeconds(moveDuration);

        // 4. Turn the player's movement scripts back on
        _fpsController.enabled = true;
        _characterController.enabled = true;

        // 5. Close the door behind them
        _animator.SetBool("IsOpen", false);

        yield return new WaitForSeconds(animationDelay);

        _isHiding = false;
        _isAnimating = false;
        _player = null; 
    }
}