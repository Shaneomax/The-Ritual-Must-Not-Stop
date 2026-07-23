using UnityEngine;

public class InteractablePickup : MonoBehaviour, IInteractable
{
    public string itemName = "Health Potion";

    public string GetDescription()
    {
        return $"Press E to pick up {itemName}";
    }

    public void Interact()
    {
        Debug.Log($"You picked up {itemName}!");
        Destroy(gameObject); // Remove item from scene
    }
}