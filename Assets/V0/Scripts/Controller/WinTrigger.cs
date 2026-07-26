using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Triggers the Win Sequence (Fade to black -> Load Credits) as soon as this GameObject becomes active.
/// Attach this to the final object that appears when the player completes the ritual (e.g. DemonBook3 in the pentagram).
/// </summary>
public class WinTrigger : MonoBehaviour
{
    [Header("Win Sequence Settings")]
    [Tooltip("The name of your Credit scene.")]
    public string creditSceneName = "CreditScene";
    
    [Tooltip("The black UI image used to fade the screen. Leave empty to jump instantly.")]
    public Image fadeScreen;

    private void Start()
    {
        if (fadeScreen != null)
        {
            // Ensure it starts transparent and hidden so it doesn't block vision before winning
            fadeScreen.color = new Color(0, 0, 0, 0);
            fadeScreen.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        // OnEnable can fire before Start() sometimes, so we don't rely on variables set in Start().
        TriggerWin();
    }

    private void TriggerWin()
    {
        Debug.Log("<color=green>WIN CONDITION MET!</color> Fading to credits...");

        if (fadeScreen != null)
        {
            fadeScreen.gameObject.SetActive(true);
            
            // Use DOTween to fade to black over 1.5 seconds, then load the scene
            fadeScreen.DOFade(1f, 1.5f).OnComplete(() => 
            {
                LoadCreditScene();
            });
        }
        else
        {
            LoadCreditScene();
        }
    }

    private void LoadCreditScene()
    {
        if (!string.IsNullOrEmpty(creditSceneName))
        {
            SceneManager.LoadScene(creditSceneName);
        }
        else
        {
            Debug.LogError("[WinTrigger] Credit Scene Name is not set!");
        }
    }
}
