using UnityEngine;

/// <summary>
/// A trigger box that fires a cinematic sequence exactly once.
/// Can be conditioned to only fire based on a GameTask's progress.
///
/// Inspector setup:
///   1. Ensure this GameObject has a Collider with IsTrigger = true.
///   2. Fill in the Base Sequence Settings (Player Context, Sequence Camera).
///   3. Set the Trigger Condition (e.g., BeforeTaskCompleted).
/// </summary>
[RequireComponent(typeof(Collider))]
public class TriggerCinematicSequence : CinematicSequenceBase
{
    public enum TriggerTaskCondition
    {
        AlwaysFire,
        BeforeTaskCompleted,
        AfterTaskCompleted
    }

    [Header("Trigger Conditions")]
    [Tooltip("When should this trigger be allowed to fire?")]
    [SerializeField] private TriggerTaskCondition _condition = TriggerTaskCondition.AlwaysFire;
    
    [Tooltip("The task to check against. (Leave null if Condition is AlwaysFire).")]
    [SerializeField] private GameTask _targetTask;

    private bool _hasFired = false;

    private void OnTriggerEnter(Collider other)
    {
        // 1. Guard checks
        if (_hasFired || !other.CompareTag("Player")) return;

        // 2. Evaluate condition
        bool conditionMet = false;
        switch (_condition)
        {
            case TriggerTaskCondition.AlwaysFire:
                conditionMet = true;
                break;
                
            case TriggerTaskCondition.BeforeTaskCompleted:
                conditionMet = (_targetTask != null && !_targetTask.isCompleted);
                break;
                
            case TriggerTaskCondition.AfterTaskCompleted:
                conditionMet = (_targetTask != null && _targetTask.isCompleted);
                break;
        }

        // 3. Fire sequence
        if (conditionMet)
        {
            _hasFired = true;

            // Turn off the trigger collider so it can never be hit again
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            StartSequence();
        }
    }
}
