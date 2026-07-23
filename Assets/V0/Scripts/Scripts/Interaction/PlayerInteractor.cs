using UnityEngine;
using StarterAssets;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Raycast Settings")]
    public Camera mainCamera;
    public float interactionDistance = 3f;
    public LayerMask interactableLayer;

    private StarterAssetsInputs _input;

    private void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (_input.interact)
                {
                    interactable.Interact();
                    _input.interact = false; 
                }
            }
        }
        else
        {
            if (_input.interact)
            {
                _input.interact = false;
            }
        }
    }
}