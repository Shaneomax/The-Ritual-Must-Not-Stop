using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening; // Import DOTween

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Tooltip("The target the enemy will chase (usually the Player).")]
    public Transform target;
    
    [Tooltip("Distance at which the enemy catches the player.")]
    public float catchDistance = 1.5f;

    [Tooltip("The name of your Main Menu scene. Make sure it is added in File > Build Settings!")]
    public string mainMenuSceneName = "MainMenu";

    [Tooltip("The black UI image used to fade the screen. Leave empty to just jump to the menu instantly.")]
    public Image fadeScreen;

    private NavMeshAgent agent;
    private bool isGameOver = false;

    private void Start()
    {
        // Get the NavMeshAgent component attached to this GameObject
        agent = GetComponent<NavMeshAgent>();
        
        if (target == null)
        {
            Debug.LogWarning("No target assigned to " + gameObject.name + "!");
        }

        if (fadeScreen != null)
        {
            // Ensure the image starts completely transparent
            fadeScreen.color = new Color(0, 0, 0, 0);
            // Deactivate the image object at the start so it doesn't block UI
            fadeScreen.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isGameOver || target == null || !agent.isActiveAndEnabled) return;

        // Keep the enemy from going *inside* the player's collider by stopping early
        agent.stoppingDistance = catchDistance;
        agent.SetDestination(target.position);

        // Check if the enemy is close enough to catch the player
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if (distanceToPlayer <= catchDistance)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        
        // Stop moving instantly
        agent.isStopped = true;

        if (fadeScreen != null)
        {
            // Activate the fade screen object
            fadeScreen.gameObject.SetActive(true);
            
            // Use DOTween to fade to black (alpha = 1) over 1.5 seconds, then load the scene
            fadeScreen.DOFade(1f, 1.5f).OnComplete(() => 
            {
                LoadMainMenu();
            });
        }
        else
        {
            // If no fade screen is assigned, jump straight to the menu
            LoadMainMenu();
        }
    }

    private void LoadMainMenu()
    {
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError("Main Menu Scene Name is not set in EnemyAI!");
        }
    }
}


