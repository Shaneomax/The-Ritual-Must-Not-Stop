using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EventActions : MonoBehaviour
{
    [Header("Teleport Settings")]
    [Tooltip("The Player GameObject (usually PlayerCapsule).")]
    public Transform playerTransform;
    
    [Tooltip("The empty GameObject representing where the player should spawn.")]
    public Transform newSpawnPoint;

    [Header("Object Toggles")]
    [Tooltip("A list of objects you want to turn off when this event happens.")]
    public GameObject[] objectsToDisable;

    [Header("Fade Settings")]
    [Tooltip("The black UI image used to fade the screen. Leave empty for instant teleport.")]
    public Image fadeScreen;
    
    [Tooltip("How long the fade takes in seconds.")]
    public float fadeDuration = 1f;

    private void Start()
    {
        if (fadeScreen != null)
        {
            fadeScreen.color = new Color(0, 0, 0, 0);
            fadeScreen.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Call this method from your Unity Event (like On Sequence Finished).
    /// </summary>
    public void ExecuteActions()
    {
        if (fadeScreen != null)
        {
            fadeScreen.gameObject.SetActive(true);
            
            // Fade to black
            fadeScreen.DOFade(1f, fadeDuration).OnComplete(() =>
            {
                // Teleport and do other actions while screen is pitch black
                PerformInstantActions();
                
                // Fade back to transparent
                fadeScreen.DOFade(0f, fadeDuration).OnComplete(() =>
                {
                    fadeScreen.gameObject.SetActive(false);
                });
            });
        }
        else
        {
            // If no fade screen is assigned, just do everything instantly
            PerformInstantActions();
        }
    }

    private void PerformInstantActions()
    {
        // 1. Teleport the player
        if (playerTransform != null && newSpawnPoint != null)
        {
            // If the player uses a CharacterController, we MUST disable it before teleporting
            // otherwise Unity physics will snap the player back to their old position!
            CharacterController cc = playerTransform.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            
            playerTransform.position = newSpawnPoint.position;
            playerTransform.rotation = newSpawnPoint.rotation;
            
            if (cc != null) cc.enabled = true;
        }
        else
        {
            Debug.LogWarning("Player Transform or Spawn Point is missing in EventActions!");
        }

        // 2. Clear Inventory
        if (playerTransform != null)
        {
            PlayerInventory inv = playerTransform.GetComponent<PlayerInventory>();
            if (inv != null)
            {
                inv.ClearInventory();
            }
        }

        // 3. Disable the requested objects
        if (objectsToDisable != null)
        {
            foreach (GameObject obj in objectsToDisable)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}

