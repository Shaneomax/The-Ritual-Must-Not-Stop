using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Trigger zone that fires a cinematic event sequence when the player enters,
/// conditioned on a GameTask's state.
///
/// Inspector setup:
///   - Player Context     → drag PlayerCapsule here
///   - Target Task        → the GameTask SO to check
///   - Cinematic Cameras  → list of Cinemachine cameras to activate during the sequence
///   - On Event Triggered → hook up any extra Unity Events here
/// </summary>
public class TaskEventSequence : MonoBehaviour
{
    public enum TaskActivationCondition
    {
        MustBeCompleted,
        MustBeStarted
    }

    [Header("References")]
    [SerializeField] private PlayerContext _playerContext;

    [Header("Task Settings")]
    [SerializeField] private GameTask _targetTask;
    [SerializeField] private TaskActivationCondition _condition = TaskActivationCondition.MustBeCompleted;

    [Header("Cinematic Sequence")]
    [Tooltip("All cameras in this list activate at the start of the sequence.")]
    [SerializeField] private List<GameObject> _cinematicCameras = new List<GameObject>();
    [SerializeField] private float _sequenceDuration = 2.5f;
    [SerializeField] private float _cameraBlendTime  = 1.0f;

    [Header("Custom Events")]
    public UnityEvent OnEventTriggered;

    private bool _hasTriggered = false;

    // ── Trigger ───────────────────────────────────────────────────────────────

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || _hasTriggered || _targetTask == null) return;

        bool conditionMet = _condition == TaskActivationCondition.MustBeCompleted
            ? _targetTask.isCompleted
            : !_targetTask.isCompleted;

        if (conditionMet) StartCoroutine(PlayEventSequence());
    }

    // ── Coroutine ─────────────────────────────────────────────────────────────

    private IEnumerator PlayEventSequence()
    {
        _hasTriggered = true;

        _playerContext.FreezePlayer();
        OnEventTriggered.Invoke();
        Debug.Log($"[Event System] Task event triggered: {_targetTask.taskName}");

        foreach (GameObject cam in _cinematicCameras)
            cam?.SetActive(true);

        yield return new WaitForSeconds(_sequenceDuration);

        foreach (GameObject cam in _cinematicCameras)
            cam?.SetActive(false);

        yield return new WaitForSeconds(_cameraBlendTime);

        _playerContext.UnfreezePlayer();
    }
}