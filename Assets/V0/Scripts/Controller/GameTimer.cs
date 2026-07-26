using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Listens to the PlayerInventory for when a specific item is picked up (e.g. Journal)
/// and starts a countdown timer. The timer duration and events are exposed in the Inspector.
/// </summary>
public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("The time limit in minutes.")]
    [SerializeField] private float _timeLimitMinutes = 10f;
    
    [Tooltip("The item that triggers the timer to start.")]
    [SerializeField] private ItemType _triggerItem = ItemType.Journal;

    [Header("UI")]
    [Tooltip("Optional: Drag your TextMeshPro UI element here to display the timer.")]
    [SerializeField] private TextMeshProUGUI _timerText;

    [Header("Dependencies")]
    [Tooltip("Reference to the player's inventory.")]
    [SerializeField] private PlayerInventory _inventory;

    [Header("Events")]
    public UnityEvent OnTimerStarted;
    public UnityEvent OnTimerExpired;

    private bool _timerRunning = false;
    private float _timeRemaining;

    private void Start()
    {
        if (_inventory != null)
        {
            _inventory.OnItemAdded += HandleItemAdded;
        }
        else
        {
            Debug.LogWarning("[GameTimer] No PlayerInventory assigned in the Inspector!");
        }

        // Initialize UI if available
        if (_timerText != null)
        {
            _timeRemaining = _timeLimitMinutes * 60f;
            UpdateTimerUI();
            
            // Hide the timer UI at start
            _timerText.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (_inventory != null)
        {
            _inventory.OnItemAdded -= HandleItemAdded;
        }
    }

    private void HandleItemAdded(ItemType type)
    {
        if (type == _triggerItem && !_timerRunning)
        {
            StartTimer();
        }
    }

    /// <summary>
    /// Call this to manually start the timer if you don't want to rely on the item pickup.
    /// </summary>
    public void StartTimer()
    {
        if (_timerRunning) return;

        _timeRemaining = _timeLimitMinutes * 60f;
        _timerRunning = true;
        
        Debug.Log($"[GameTimer] Timer started! You have {_timeLimitMinutes} minutes remaining.");
        
        // Show the timer UI
        if (_timerText != null)
        {
            _timerText.gameObject.SetActive(true);
        }

        UpdateTimerUI();
        OnTimerStarted?.Invoke();
    }

    private void Update()
    {
        if (!_timerRunning) return;

        _timeRemaining -= Time.deltaTime;

        if (_timeRemaining <= 0f)
        {
            _timerRunning = false;
            _timeRemaining = 0f;
            
            Debug.Log("[GameTimer] Time is up!");
            UpdateTimerUI();
            OnTimerExpired?.Invoke();
        }
        else
        {
            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        if (_timerText == null) return;

        int minutes = Mathf.FloorToInt(_timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(_timeRemaining % 60f);
        
        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// Useful for other scripts to read the remaining time.
    /// </summary>
    public float GetTimeRemainingSeconds()
    {
        return _timeRemaining;
    }
}
