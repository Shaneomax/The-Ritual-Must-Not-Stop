using UnityEngine;

public class InteractableCandle : MonoBehaviour, IInteractable
{
    [Header("Candle Settings")]
    [Tooltip("Drag the Particle System child object here")]
    public ParticleSystem candleFlame;
    public bool isLit = false;

    [Header("Player Settings")]
    [Tooltip("The exact name of the Lighter GameObject on the player")]
    public string lighterObjectName = "Lighter"; 

    public string GetDescription()
    {
        if (isLit) 
        {
            return "The candle is burning.";
        }
        
        // Updated text to reflect both possibilities
        return "Press E to Light (with Lighter) or Pick Up";
    }

    public void Interact()
    {
        // If it's already lit, do nothing
        if (isLit) return; 

        // Check if the player has the lighter active in their hands
        if (IsLighterActive())
        {
            LightCandle();
        }
        else
        {
            // If the lighter is NOT active, pick it up instead
            PickUpCandle();
        }
    }

    private bool IsLighterActive()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Transform[] allChildren = player.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if (child.name == lighterObjectName)
                {
                    return child.gameObject.activeInHierarchy;
                }
            }
        }
        return false;
    }

    private void LightCandle()
    {
        isLit = true;
        
        if (candleFlame != null)
        {
            ParticleSystem.EmissionModule emission = candleFlame.emission;
            emission.enabled = true;
            candleFlame.Play(); 
        }
        
        Debug.Log("Candle lit!");
    }

    private void PickUpCandle()
    {
        Debug.Log("You picked up the unlit candle!");
        
        // Note: If you add an inventory system later (like the ScriptableObjects we discussed), 
        // you will want to add the code to store the item in your inventory right here, 
        // just before destroying the object.
        
        Destroy(gameObject);
    }
}