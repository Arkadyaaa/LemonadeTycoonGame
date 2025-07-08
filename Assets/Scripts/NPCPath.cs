using UnityEngine;
using UnityEngine.AI;

public abstract class NPCPath : MonoBehaviour
{
    public bool ActivePath = false;
    protected bool pathStarted = false;
    protected bool onEndPath = false;

    protected Transform midpoint;
    protected Transform end;
    protected Transform shop;

    protected NavMeshAgent agent;
    protected NPCMovement npcMovement;


    public void SetPath(Transform midpoint, Transform end, Transform shop, NPCMovement npcMovement)
    {
        this.midpoint = midpoint;
        this.end = end;
        this.shop = shop;

        this.npcMovement = npcMovement;
        this.agent = npcMovement.agent;

        SetDestination();
    }
    
    protected abstract void SetDestination();
}
