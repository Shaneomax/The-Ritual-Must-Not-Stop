using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Unity.Cinemachine;

/// <summary>
/// Abstract base class for all cinematic sequences in the game.
/// Handles freezing the player, boosting camera priority, waiting, and unfreezing seamlessly.
/// 
/// Inherit from this and call StartSequence() whenever your specific trigger condition is met.
/// </summary>
public abstract class CinematicSequenceBase : MonoBehaviour
{
    [Header("Base Sequence Settings")]
    [SerializeField] protected PlayerContext _playerContext;
    
    [Tooltip("The camera to activate for this sequence.")]
    [SerializeField] protected CinemachineCamera _sequenceCamera;

    [Tooltip("Priority while active (should be higher than Player Camera).")]
    [SerializeField] protected int _activePriority = 20;
    
    [Tooltip("Priority after finishing (should be lower than Player Camera).")]
    [SerializeField] protected int _inactivePriority = 0;

    [Header("Sequence Timing")]
    [Tooltip("How long the cinematic camera stays active before giving control back.")]
    [SerializeField] protected float _sequenceDuration = 2.5f;
    
    [Tooltip("How long the blend back to the player takes.")]
    [SerializeField] protected float _blendToPlayerDuration = 1.5f;

    [Header("Smooth Hand-off")]
    [Tooltip("If true, syncs the player's rotation to the cinematic camera right before the blend ends, preventing neck-snapping.")]
    [SerializeField] protected bool _syncRotationAtEnd = true;

    [Header("Base Events")]
    public UnityEvent OnSequenceStarted;
    public UnityEvent OnSequenceFinished;

    protected bool _isRunning = false;

    /// <summary>
    /// Call this from child classes (e.g., inside OnTriggerEnter) to play the sequence.
    /// </summary>
    protected void StartSequence()
    {
        if (_isRunning) return;
        
        if (_playerContext == null || _sequenceCamera == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Sequence failed to start: Missing PlayerContext or SequenceCamera.");
            return;
        }

        StartCoroutine(SequenceRoutine());
    }

    private IEnumerator SequenceRoutine()
    {
        _isRunning = true;
        
        // 1. Freeze player & Fire events
        _playerContext.FreezePlayer();
        OnSequenceStarted?.Invoke();

        // 2. Boost priority so Cinemachine takes over
        _sequenceCamera.Priority = _activePriority;

        // 3. Wait for the cinematic duration
        yield return new WaitForSeconds(_sequenceDuration);

        // 4. Sync look direction to prevent snapping
        if (_syncRotationAtEnd)
        {
            _playerContext.SyncLookToCamera(_sequenceCamera.transform);
        }

        // 5. Drop priority so Cinemachine blends back to the player
        _sequenceCamera.Priority = _inactivePriority;

        // 6. Wait for the physical blend to finish
        yield return new WaitForSeconds(_blendToPlayerDuration);

        // 7. Unfreeze player
        _playerContext.UnfreezePlayer();
        OnSequenceFinished?.Invoke();
        
        _isRunning = false;
    }
}
