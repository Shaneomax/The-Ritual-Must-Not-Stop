using UnityEngine;

public class InteractableDoor : MonoBehaviour, IInteractable
{
    public bool isOpen = false;

    public string GetDescription()
    {
        return isOpen ? "Press E to Close Door" : "Press E to Open Door";
    }

    public void Interact()
    {
        isOpen = !isOpen;
        Debug.Log(isOpen ? "Door Opened!" : "Door Closed!");
        // Add your Animator or Transform rotation logic here
    }
}