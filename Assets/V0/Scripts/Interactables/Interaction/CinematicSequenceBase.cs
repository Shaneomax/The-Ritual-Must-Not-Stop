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
    
    [Tooltip("The camera to activate for this sequence. (Optional)")]
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

    [System.Serializable]
    public struct TimedSequenceEvent
    {
        [Tooltip("Time in seconds from the START of the sequence to trigger this event.")]
        public float TriggerTime;
        public UnityEvent OnEventTriggered;
    }

    [Header("Mid-Sequence Events")]
    [Tooltip("Events that trigger at specific times while the sequence is playing.")]
    public TimedSequenceEvent[] MidSequenceEvents;

    protected bool _isRunning = false;

    /// <summary>
    /// Call this from child classes (e.g., inside OnTriggerEnter) to play the sequence.
    /// </summary>
    protected void StartSequence()
    {
        if (_isRunning) return;
        
        if (_playerContext == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Sequence failed to start: Missing PlayerContext.");
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

        // Start processing mid-sequence timed events in parallel
        if (MidSequenceEvents != null && MidSequenceEvents.Length > 0)
        {
            StartCoroutine(HandleTimedEvents());
        }

        if (_sequenceCamera != null)
        {
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
        }
        else
        {
            // If no camera is provided, just wait for the duration like a normal timer
            if (_sequenceDuration > 0)
            {
                yield return new WaitForSeconds(_sequenceDuration);
            }
        }

        // 7. Unfreeze player
        _playerContext.UnfreezePlayer();
        OnSequenceFinished?.Invoke();
        
        _isRunning = false;
    }

    private IEnumerator HandleTimedEvents()
    {
        float timer = 0f;
        bool[] hasFired = new bool[MidSequenceEvents.Length];

        // Keep checking the timer as long as the sequence is active
        while (_isRunning)
        {
            for (int i = 0; i < MidSequenceEvents.Length; i++)
            {
                if (!hasFired[i] && timer >= MidSequenceEvents[i].TriggerTime)
                {
                    hasFired[i] = true;
                    MidSequenceEvents[i].OnEventTriggered?.Invoke();
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }
}

