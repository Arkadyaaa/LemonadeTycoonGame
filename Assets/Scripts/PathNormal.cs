using UnityEngine;
using UnityEngine.AI;

public class PathNormal : NPCPath
{
    void Update()
    {
        if (!pathStarted) return;

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if ((!agent.hasPath || agent.velocity.sqrMagnitude < 2f) && !onEndPath)
                {
                    agent.SetDestination(end.position);
                    onEndPath = true;
                }
                else if ((!agent.hasPath || agent.velocity.sqrMagnitude < 2f) && onEndPath)
                {
                    npcMovement.SafelyDestroy();
                }
            }
        }
    }

    protected override void SetDestination()
    {
        agent.SetDestination(midpoint.position);
        pathStarted = true;
    }
}
