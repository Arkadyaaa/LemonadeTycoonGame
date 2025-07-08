using UnityEngine;
using UnityEngine.AI;

public class NPCBehavior : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    // Force delete incase the NPC doesnt get deleted on endpoint
    private float lifetimeTimer = 0f;
    private float maxLifetime = 200f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Find Animator on the child (the model)
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogWarning($"Animator not found in children of {gameObject.name}");

        if (agent == null)
            Debug.LogWarning($"NavMeshAgent not found on {gameObject.name}");
    }

    void Update()
    {
        if (animator != null && agent != null)
        {
            bool isMoving = agent.velocity.sqrMagnitude > 0.01f;
            animator.SetBool("isWalking", isMoving);
        }

        // Make the npc crumble and die of old age
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer >= maxLifetime)
        {
            Destroy(gameObject);
            return;
        }
    }
}
