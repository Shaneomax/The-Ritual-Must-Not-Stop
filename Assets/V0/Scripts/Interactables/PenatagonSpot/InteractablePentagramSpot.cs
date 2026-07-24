using System.Collections;
using UnityEngine;
using StarterAssets; 

public class InteractablePentagramSpot : MonoBehaviour, IInteractable
{
    [Header("Pentagram Target")]
    public GameObject pentagramObject;
    [Tooltip("How long to wait after drawing before the camera pans back up")]
    public float drawDelay = 1.0f;
    
    [Header("Cinemachine Settings")]
    [Tooltip("Drag the PentagramCam GameObject here")]
    public GameObject lookDownCamera; 
    [Tooltip("How long the camera sweep takes (match your Cinemachine Brain default blend)")]
    public float cameraBlendTime = 1.5f;
    
    [Header("Player Settings")]
    public string chalkObjectName = "Chalk"; 

    private bool _isDrawn = false;
    private bool _isPlayerInside = false;
    private bool _isAnimating = false;

    private void Start()
    {
        if (pentagramObject != null)
        {
            pentagramObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) _isPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) _isPlayerInside = false;
    }

    public string GetDescription()
    {
        if (_isDrawn) return "The pentagram is drawn.";
        if (_isAnimating) return ""; 
        if (!_isPlayerInside) return "Get closer to draw the Pentagram.";
        
        return "Press E to Draw Pentagram (Requires Chalk)";
    }

    public void Interact()
    {
        if (_isDrawn || !_isPlayerInside || _isAnimating) return;

        if (IsChalkActive())
        {
            StartCoroutine(DrawPentagramSequence());
        }
        else
        {
            Debug.Log("You need active chalk in your hand to draw this!");
        }
    }

    private bool IsChalkActive()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Transform[] allChildren = player.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if (child.name == chalkObjectName)
                {
                    return child.gameObject.activeInHierarchy;
                }
            }
        }
        return false;
    }

    private IEnumerator DrawPentagramSequence()
    {
        _isAnimating = true;

        // 1. Grab Player References
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        FirstPersonController fpsController = player.GetComponent<FirstPersonController>();
        CharacterController cc = player.GetComponent<CharacterController>();

        // 2. Disable movement and looking
        if (fpsController != null) fpsController.enabled = false;
        if (cc != null) cc.enabled = false;

        // 3. Switch to the Look Down Camera
        if (lookDownCamera != null) lookDownCamera.SetActive(true);

        // Wait for Cinemachine to smoothly sweep the camera down
        yield return new WaitForSeconds(cameraBlendTime);

        // 4. Instantly draw the Pentagram
        if (pentagramObject != null)
        {
            pentagramObject.SetActive(true);
        }
        
        // Give the player a moment to see the drawing before snapping back
        yield return new WaitForSeconds(drawDelay); 

        // 5. Turn off the Look Down Camera (Cinemachine automatically sweeps back to the player)
        if (lookDownCamera != null) lookDownCamera.SetActive(false);

        // Wait for Cinemachine to sweep back
        yield return new WaitForSeconds(cameraBlendTime);

        // 6. Re-enable inputs seamlessly
        if (fpsController != null) fpsController.enabled = true;
        if (cc != null) cc.enabled = true;

        _isDrawn = true;
        _isAnimating = false;
        Debug.Log("Pentagram drawn successfully!");
    }
}