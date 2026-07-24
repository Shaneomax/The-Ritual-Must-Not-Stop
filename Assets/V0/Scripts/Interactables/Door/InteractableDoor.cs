using UnityEngine;

public class InteractableDoor : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public bool isOpen = false;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public string GetDescription()
    {
        return isOpen ? "Press E to Close Door" : "Press E to Open Door";
    }

    public void Interact()
    {
        isOpen = !isOpen; 
        Debug.Log(isOpen ? "Door Opened!" : "Door Closed!");
        if (_animator != null)
        {
            _animator.SetBool("IsOpen", isOpen);
        }
    }
}