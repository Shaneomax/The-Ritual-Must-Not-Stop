using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using StarterAssets;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class JournalUIController : MonoBehaviour
{
    [System.Serializable]
    public struct RitualTaskItem
    {
        public ItemType itemType;
        public string description;
    }

    [Header("References")]
    [Tooltip("The panel you created in the Canvas for the Journal.")]
    public GameObject journalPanel;

    [Tooltip("Reference to the player's inventory.")]
    public PlayerInventory playerInventory;
    
    [Tooltip("Reference to the StarterAssets inputs (needed to unlock the mouse).")]
    public StarterAssetsInputs playerInputs;

    [Header("Checklist UI")]
    [Tooltip("TextMeshPro text element displaying the checklist on the Journal page.")]
    public TMP_Text checklistText;

    [Tooltip("Legacy UnityEngine.UI.Text fallback component if TextMeshPro is not used.")]
    public Text legacyChecklistText;

    [Tooltip("Header text displayed at the top of the ritual items checklist.")]
    [TextArea(2, 4)]
    public string checklistHeader = "Ritual items\n------------------------------";

    [Tooltip("List of ritual tasks tracked in the journal.")]
    public List<RitualTaskItem> ritualTasks = new List<RitualTaskItem>()
    {
        new RitualTaskItem { itemType = ItemType.DemonBook, description = "Find Demon book" },
        new RitualTaskItem { itemType = ItemType.Candle, description = "Collect candle" },
        new RitualTaskItem { itemType = ItemType.Chalk, description = "Collect chalk" },
        new RitualTaskItem { itemType = ItemType.Skull, description = "Find Skull" }
    };

    private bool _isJournalOpen = false;

    private void Start()
    {
        // Auto-find references if missing
        if (playerInventory == null)
        {
            playerInventory = FindAnyObjectByType<PlayerInventory>();
        }

        if (playerInputs == null)
        {
            playerInputs = FindAnyObjectByType<StarterAssetsInputs>();
        }

        // Subscribe to inventory changes to refresh checklist dynamically
        if (playerInventory != null)
        {
            playerInventory.OnItemAdded += HandleItemAdded;
        }

        // Ensure the journal starts closed
        if (journalPanel != null)
        {
            journalPanel.SetActive(false);
        }

        UpdateChecklistUI();
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.OnItemAdded -= HandleItemAdded;
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

    private void HandleItemAdded(ItemType type)
    {
        UpdateChecklistUI();
    }

    /// <summary>
    /// Updates the checklist text component based on collected inventory items.
    /// </summary>
    public void UpdateChecklistUI()
    {
        StringBuilder sb = new StringBuilder();
        if (!string.IsNullOrEmpty(checklistHeader))
        {
            sb.AppendLine(checklistHeader);
        }

        for (int i = 0; i < ritualTasks.Count; i++)
        {
            RitualTaskItem task = ritualTasks[i];
            bool isCollected = playerInventory != null && playerInventory.HasItem(task.itemType);
            string checkMark = isCollected ? "[✓]" : "[  ]";
            sb.AppendLine($"{checkMark} {i + 1}. {task.description}");
        }

        string fullText = sb.ToString().TrimEnd();

        if (checklistText != null)
        {
            checklistText.text = fullText;
        }

        if (legacyChecklistText != null)
        {
            legacyChecklistText.text = fullText;
        }
    }

    public void ToggleJournal()
    {
        Debug.Log("J was pressed! Checking if we can open the journal...");

        if (playerInventory == null)
        {
            playerInventory = FindAnyObjectByType<PlayerInventory>();
        }

        if (playerInventory == null)
        {
            Debug.LogError("ERROR: Player Inventory is not assigned in the Inspector and could not be found!");
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

        // Toggle the state
        _isJournalOpen = !_isJournalOpen;

        if (_isJournalOpen)
        {
            UpdateChecklistUI();
        }

        journalPanel.SetActive(_isJournalOpen);

        // Handle Time and Mouse Cursor
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
