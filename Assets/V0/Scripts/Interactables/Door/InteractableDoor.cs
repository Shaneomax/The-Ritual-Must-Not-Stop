using UnityEngine;

public class InteractableDoor : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public bool isOpen = false;
    private Animator _animator;

    [Header("Lock Settings")]
    [Tooltip("The exact name of the Key GameObject on the player")]
    public string keyObjectName = "Key";

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public string GetDescription()
    {
        if (isOpen)
        {
            return "Press E to Close Door";
        }
        
        // Change the UI prompt if the door is tagged as Locked
        if (gameObject.CompareTag("Locked"))
        {
            return "Press E to Unlock Door (Requires Key)";
        }

        return "Press E to Open Door";
    }

    public void Interact()
    {
        // 1. If the door is closed and locked, we must check for the key first
        if (!isOpen && gameObject.CompareTag("Locked"))
        {
            if (IsKeyActive())
            {
                UnlockDoor();
            }
            else
            {
                Debug.Log("The door is locked. You need a key!");
                return; // Stop the interaction here so the door doesn't open
            }
        }

        // 2. Standard open/close logic (runs if unlocked, or if they just unlocked it)
        isOpen = !isOpen; 
        Debug.Log(isOpen ? "Door Opened!" : "Door Closed!");
        
        if (_animator != null)
        {
            _animator.SetBool("IsOpen", isOpen);
        }
    }

    private void UnlockDoor()
    {
        Debug.Log("Door unlocked with the key!");
        
        // Remove the Locked tag so the player doesn't need to keep holding the key
        // to close or re-open this door later.
        gameObject.tag = "Untagged"; 
    }

    private bool IsKeyActive()
    {
        // Search the player for the active Key object
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Transform[] allChildren = player.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if (child.name == keyObjectName)
                {
                    return child.gameObject.activeInHierarchy;
                }
            }
        }
        return false;
    }
}