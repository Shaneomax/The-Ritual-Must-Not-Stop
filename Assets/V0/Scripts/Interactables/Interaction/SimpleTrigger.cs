using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class SimpleTrigger : MonoBehaviour
{
    [Tooltip("The tag of the object that can trigger this event (usually Player).")]
    public string targetTag = "Player";
    
    [Tooltip("If true, the trigger will only happen the first time the player walks into it.")]
    public bool triggerOnlyOnce = true;
    private bool hasTriggered = false;

    [Space(10)]
    public UnityEvent onTriggerEnterEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnlyOnce && hasTriggered) return;

        if (other.CompareTag(targetTag))
        {
            hasTriggered = true;
            onTriggerEnterEvent.Invoke();
        }
    }
}
