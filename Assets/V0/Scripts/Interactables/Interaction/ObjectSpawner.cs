using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A modular script to activate a hidden GameObject (like a Demon Book) 
/// with an optional delay. Trigger this via OnSequenceFinished!
/// </summary>
public class ObjectSpawner : MonoBehaviour
{
    [Header("Activation Settings")]
    [Tooltip("The disabled GameObject in your scene that should be turned on.")]
    [SerializeField] private GameObject _objectToActivate;

    [Header("Optional Effects")]
    [Tooltip("Delay in seconds before the object turns on after being triggered.")]
    [SerializeField] private float _delay = 0f;

    [Tooltip("Fired immediately after the object is activated.")]
    public UnityEvent OnActivated;

    private bool _hasTriggered = false;

    /// <summary>
    /// Activates the target object. Call this from UnityEvents.
    /// </summary>
    public void Spawn()
    {
        if (_hasTriggered || _objectToActivate == null) return;
        
        if (_delay > 0f)
            StartCoroutine(ActivateWithDelay());
        else
            ExecuteActivation();
    }

    private void ExecuteActivation()
    {
        _hasTriggered = true;
        _objectToActivate.SetActive(true);
        OnActivated?.Invoke();
    }

    private IEnumerator ActivateWithDelay()
    {
        yield return new WaitForSeconds(_delay);
        ExecuteActivation();
    }
}
