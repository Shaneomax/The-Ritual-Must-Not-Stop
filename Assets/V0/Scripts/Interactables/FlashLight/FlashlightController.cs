using UnityEngine;
using StarterAssets;

public class FlashlightController : MonoBehaviour
{
    [Header("Flashlight Settings")]
    [Tooltip("Drag your Spot Light here")]
    public Light flashlightLight;
    public bool isLit = false;

    private StarterAssetsInputs _input;

    private void Start()
    {
        // Find the input script on the Player
        _input = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssetsInputs>();

        // Ensure the light matches the starting state
        if (flashlightLight != null)
        {
            flashlightLight.enabled = isLit;
        }
    }

    private void Update()
    {
        // If the player pressed the 'F' key
        if (_input != null && _input.flashlight)
        {
            ToggleFlashlight();
            
            // Instantly consume the input so it doesn't flicker on and off every frame
            _input.flashlight = false; 
        }
    }

    private void ToggleFlashlight()
    {
        isLit = !isLit;
        
        if (flashlightLight != null)
        {
            flashlightLight.enabled = isLit;
        }

        Debug.Log(isLit ? "Flashlight ON" : "Flashlight OFF");
    }
}