using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using StarterAssets;
// using UnityEngine.Audio;

public class PauseManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Player Input Reference")]
    [SerializeField] private StarterAssetsInputs playerInputs;

    // [Header("Audio Settings")]
    // [SerializeField] private AudioMixer mainAudioMixer;

    private bool isPaused = false;

    private void Start()
    {
        ResumeGame();

        if (playerInputs == null)
        {
            playerInputs = FindAnyObjectByType<StarterAssetsInputs>();
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettings();
            }
            else
            {
                ResumeGame();
            }
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pausePanel != null) pausePanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        Time.timeScale = 0f;

        if (playerInputs != null)
        {
            playerInputs.SetPauseState(true);
            playerInputs.cursorLocked = false;
            playerInputs.cursorInputForLook = false;
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        Time.timeScale = 1f;

        if (playerInputs != null)
        {
            playerInputs.SetPauseState(false);
            playerInputs.cursorLocked = true;
            playerInputs.cursorInputForLook = true;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSettings()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    // public void SetMusicVolume(float sliderValue)
    // {
    //     if (mainAudioMixer != null)
    //     {
    //         mainAudioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20);
    //     }
    // }

    // public void SetSFXVolume(float sliderValue)
    // {
    //     if (mainAudioMixer != null)
    //     {
    //         mainAudioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20);
    //     }
    // }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}