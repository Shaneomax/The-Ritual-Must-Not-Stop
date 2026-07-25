using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Unity.Cinemachine; // Cinemachine 3 namespace
using DG.Tweening; // Added for DOTween

/// <summary>
/// Plays a Cinemachine transition at the start of the game (e.g., waking up or standing up).
/// Freezes the player, boosts the starting camera's priority, waits, then lowers it to blend
/// back to the player camera, and finally gives the player control.
/// </summary>
public class GameStartSequence : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerContext _playerContext;
    
    [Tooltip("The camera the player starts looking through (e.g., a sitting camera).")]
    [SerializeField] private CinemachineCamera _startingCamera;

    [Header("Priority Settings")]
    [Tooltip("Priority when the sequence starts (must be higher than Player Camera).")]
    [SerializeField] private int _activePriority = 20;
    
    [Tooltip("Priority after the sequence finishes (must be lower than Player Camera).")]
    [SerializeField] private int _inactivePriority = 0;

    [Header("Wake Up Animation (DOTween)")]
    [Tooltip("Forces the starting camera to (0,0,0) and animates it to a target rotation. Ensure Cinemachine Aim is set to 'Do Nothing'.")]
    [SerializeField] private bool _useWakeUpAnimation = true;
    
    [Tooltip("The target rotation the camera will slowly turn towards when waking up.")]
    [SerializeField] private Vector3 _wakeUpTargetRotation = new Vector3(-15f, 0f, 0f); // Slight look up
    
    [Tooltip("How long the wake-up head movement takes.")]
    [SerializeField] private float _wakeUpDuration = 2.0f;

    [Header("Timing")]
    [Tooltip("How long to stay completely still before the wake-up animation starts.")]
    [SerializeField] private float _initialWaitDuration = 1.0f;
    
    [Tooltip("How long the Cinemachine blend back to the player takes. Match this to your Cinemachine Brain setting.")]
    [SerializeField] private float _blendToPlayerDuration = 1.5f;

    [Header("Optional Events")]
    public UnityEvent OnSequenceStarted;
    public UnityEvent OnSequenceFinished;

    private void Awake()
    {
        // 1. Freeze player and boost priority IMMEDIATELY in Awake
        if (_playerContext != null)
            _playerContext.FreezePlayer();

        if (_startingCamera != null)
        {
            _startingCamera.Priority = _activePriority;
            // Removed: The camera will now naturally start from whatever rotation you set in the Editor.
        }
    }

    private void Start()
    {
        if (_playerContext != null && _startingCamera != null)
        {
            StartCoroutine(PlayStartSequence());
        }
        else
        {
            Debug.LogWarning("[GameStartSequence] Missing PlayerContext or StartingCamera reference.");
        }
    }

    private IEnumerator PlayStartSequence()
    {
        OnSequenceStarted?.Invoke();

        // 2. Wait completely still (e.g. eyes closed / black screen)
        if (_initialWaitDuration > 0f)
            yield return new WaitForSeconds(_initialWaitDuration);

        // 3. Wake up animation (DOTween)
        if (_useWakeUpAnimation)
        {
            _startingCamera.transform.DORotate(_wakeUpTargetRotation, _wakeUpDuration)
                                     .SetEase(Ease.InOutSine);
            
            // Wait for the animation to finish looking around/up
            yield return new WaitForSeconds(_wakeUpDuration);
        }

        // 4. Sync player's internal rotation to exactly match the sitting camera!
        _playerContext.SyncLookToCamera(_startingCamera.transform);

        // 5. Drop the priority (Starts the stand-up blend)
        _startingCamera.Priority = _inactivePriority;

        // 6. Wait for the blend to finish
        yield return new WaitForSeconds(_blendToPlayerDuration);

        // 7. Give the player control
        _playerContext.UnfreezePlayer();
        OnSequenceFinished?.Invoke();
        
        Debug.Log("[GameStartSequence] Sequence finished, player has control.");
    }
}


