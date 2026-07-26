using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A modular script to activate hidden GameObjects (like a Demon Book) 
/// with an optional delay. Trigger this via OnSequenceFinished!
/// </summary>
public class ObjectSpawner : MonoBehaviour
{
    [Header("Activation Settings")]
    [Tooltip("The disabled GameObjects in your scene that should be turned on.")]
    [SerializeField] private List<GameObject> _objectsToActivate = new List<GameObject>();

    [Header("Optional Effects")]
    [Tooltip("Delay in seconds before the objects turn on after being triggered.")]
    [SerializeField] private float _delay = 0f;

    [Tooltip("Fired immediately after the objects are activated.")]
    public UnityEvent OnActivated;

    private bool _hasTriggered = false;

    /// <summary>
    /// Activates the target objects. Call this from UnityEvents.
    /// </summary>
    public void Spawn()
    {
        if (_hasTriggered || _objectsToActivate == null || _objectsToActivate.Count == 0) return;
        
        if (_delay > 0f)
            StartCoroutine(ActivateWithDelay());
        else
            ExecuteActivation();
    }

    private void ExecuteActivation()
    {
        _hasTriggered = true;

        foreach (GameObject obj in _objectsToActivate)
        {
            if (obj != null) 
                obj.SetActive(true);
        }

        OnActivated?.Invoke();
    }

    private IEnumerator ActivateWithDelay()
    {
        yield return new WaitForSeconds(_delay);
        ExecuteActivation();
    }
}
