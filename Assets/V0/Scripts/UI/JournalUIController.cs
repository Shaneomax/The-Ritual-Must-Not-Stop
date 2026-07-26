using UnityEngine;
using StarterAssets;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class JournalUIController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The panel you created in the Canvas for the Journal.")]
    public GameObject journalPanel;

    [Tooltip("Reference to the player's inventory.")]
    public PlayerInventory playerInventory;
    
    [Tooltip("Reference to the StarterAssets inputs (needed to unlock the mouse).")]
    public StarterAssetsInputs playerInputs;

    private bool _isJournalOpen = false;

    private void Start()
    {
        // Ensure the journal starts closed
        if (journalPanel != null)
        {
            journalPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // Check if the input system registered a Journal button press
        if (playerInputs != null && playerInputs.journal)
        {
            // Immediately set it to false so it doesn't trigger multiple times in one frame
            playerInputs.journal = false;
            
            ToggleJournal();
        }
    }

    public void ToggleJournal()
    {
        Debug.Log("J was pressed! Checking if we can open the journal...");

        if (playerInventory == null)
        {
            Debug.LogError("ERROR: Player Inventory is not assigned in the Inspector!");
            return;
        }

        if (!playerInventory.HasItem(ItemType.Journal))
        {
            Debug.Log("Failed: The player does not have the Journal item in their inventory yet!");
            return;
        }

        if (journalPanel == null)
        {
            Debug.LogError("ERROR: Journal Panel is not assigned in the Inspector!");
            return;
        }

        Debug.Log("Success! Toggling Journal Panel.");

        // 2. Toggle the state
        _isJournalOpen = !_isJournalOpen;
        journalPanel.SetActive(_isJournalOpen);

        // 3. Handle Time and Mouse Cursor
        if (_isJournalOpen)
        {
            // Pause the game time
            Time.timeScale = 0f;
            
            // Unlock the cursor so the player can click buttons on the journal (if any)
            if (playerInputs != null)
            {
                playerInputs.cursorLocked = false;
                playerInputs.cursorInputForLook = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else
        {
            // Resume the game time
            Time.timeScale = 1f;
            
            // Lock the cursor and let the player look around again
            if (playerInputs != null)
            {
                playerInputs.cursorLocked = true;
                playerInputs.cursorInputForLook = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
