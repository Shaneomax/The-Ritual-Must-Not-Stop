using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 
using StarterAssets; 

public class TaskEventSequence : MonoBehaviour
{
    public enum TaskActivationCondition
    {
        MustBeStarted,
        MustBeCompleted
    }

    [Header("Task Settings")]
    [Tooltip("The ScriptableObject task this zone listens to")]
    public GameTask targetTask;
    [Tooltip("Should this trigger work when the task starts, or only when it's completed?")]
    public TaskActivationCondition condition = TaskActivationCondition.MustBeCompleted;
    
    [Header("Cinematic Sequence Settings")]
    [Tooltip("A list of Cinemachine cameras to cycle through during the sequence")]
    public List<GameObject> cinematicCameras = new List<GameObject>();
    [Tooltip("How long each camera stays active, or total duration")]
    public float sequenceDuration = 2.5f; 
    [Tooltip("Cinemachine blend time")]
    public float cameraBlendTime = 1.0f; 
    
    [Header("Custom Events")]
    [Tooltip("Add any custom functions or debug logs here when the event fires")]
    public UnityEvent onEventTriggered;

    private bool _hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_hasTriggered && targetTask != null)
        {
            // Check condition based on what the user requested
            bool conditionMet = false;

            if (condition == TaskActivationCondition.MustBeCompleted && targetTask.isCompleted)
            {
                conditionMet = true;
            }
            else if (condition == TaskActivationCondition.MustBeStarted && !targetTask.isCompleted)
            {
                // Assuming if it's not completed yet, it's active/started
                conditionMet = true; 
            }

            if (conditionMet)
            {
                StartCoroutine(PlayEventSequence());
            }
        }
    }

    private IEnumerator PlayEventSequence()
    {
        _hasTriggered = true;

        // 1. Grab Player References and freeze them
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        FirstPersonController fpsController = player.GetComponent<FirstPersonController>();
        CharacterController cc = player.GetComponent<CharacterController>();

        if (fpsController != null) fpsController.enabled = false;
        if (cc != null) cc.enabled = false;

        // 2. Fire custom Unity Events (can be Debug.Log, spawning items, etc.)
        onEventTriggered.Invoke();
        Debug.Log($"[Event System] Task event triggered for task: {targetTask.taskName}");

        // 3. Activate the list of Cinemachine cameras sequentially or all at once
        foreach (GameObject cam in cinematicCameras)
        {
            if (cam != null) cam.SetActive(true);
        }

        // 4. Wait for the sequence duration
        yield return new WaitForSeconds(sequenceDuration);

        // 5. Turn off all cinematic cameras in the list
        foreach (GameObject cam in cinematicCameras)
        {
            if (cam != null) cam.SetActive(false);
        }

        yield return new WaitForSeconds(cameraBlendTime);

        // 6. Re-enable player movement
        if (fpsController != null) fpsController.enabled = true;
        if (cc != null) cc.enabled = true;
    }
}