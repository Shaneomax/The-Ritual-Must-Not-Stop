using UnityEngine;

public class InteractableTaskPickup : MonoBehaviour, IInteractable
{
    [Header("Task Settings")]
    [Tooltip("The ScriptableObject task this item completes")]
    public GameTask taskToComplete; 
    
    [Header("Item Details")]
    public string itemName = "Demon Book";

    public string GetDescription()
    {
        return $"Press E to pick up {itemName}";
    }

    public void Interact()
    {   
        Debug.Log($"You picked up {itemName}!");

        // Mark the ScriptableObject task as complete!
        if (taskToComplete != null)
        {
            taskToComplete.isCompleted = true;
            Debug.Log($"Task Completed: {taskToComplete.taskName}");
        }

        // Destroy the object like a normal pickup
        Destroy(gameObject); 
    }
}