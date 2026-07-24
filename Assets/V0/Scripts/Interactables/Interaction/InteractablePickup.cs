using UnityEngine;

public class InteractablePickup : MonoBehaviour, IInteractable
{
    public ItemData itemData; 

    public string GetDescription()
    {
        return $"Press E to pick up {itemData.itemName}";
    }

    public void Interact()
    {   
        Debug.Log($"You picked up {itemData.itemName}!");
        Destroy(gameObject); 
    }
}