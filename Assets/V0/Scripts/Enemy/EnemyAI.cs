using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Tooltip("The target the enemy will chase (usually the Player).")]
    public Transform target;
    
    private NavMeshAgent agent;

    private void Start()
    {
        // Get the NavMeshAgent component attached to this GameObject
        agent = GetComponent<NavMeshAgent>();
        
        if (target == null)
        {
            Debug.LogWarning("No target assigned to " + gameObject.name + "!");
        }
    }

    private void Update()
    {
        // Continuously update the destination to the target's position
        if (target != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(target.position);
        }
    }
}
