using UnityEngine;
using System.Collections.Generic;

public class ShopPOI : MonoBehaviour
{
    public Vector3 queueOffset = new Vector3(-1f, 0, 0);
    private Queue<PathCustomer> npcs = new Queue<PathCustomer>();

    public int RegisterInQueue(PathCustomer npc)
    {
        npcs.Enqueue(npc);
        return npcs.Count - 1;
    }

    public void RemoveFromQueue(PathCustomer npc)
    {
        if (npcs.Count == 0) return;

        // If the NPC is at the front, remove and update the queue
        if (npcs.Peek() == npc)
        {
            npcs.Dequeue();
        }
        else
        {
            // Rebuild the queue without this npc
            Queue<PathCustomer> newQueue = new Queue<PathCustomer>();
            foreach (var n in npcs)
            {
                if (n != npc)
                    newQueue.Enqueue(n);
            }
            npcs = newQueue;
        }

        // Reposition all NPCs
        PathCustomer[] currentNpcs = npcs.ToArray();
        for (int i = 0; i < currentNpcs.Length; i++)
        {
            currentNpcs[i].UpdateQueuePosition(GetQueuePosition(currentNpcs[i]));
        }
    }

    public Vector3 GetQueuePosition(PathCustomer npc)
    {
        return transform.position + queueOffset * GetQueueIndex(npc);
    }

    public int GetQueueIndex(PathCustomer npc)
    {
        int index = 0;
        foreach (var n in npcs)
        {
            if (n == npc)
                return index;
            index++;
        }
        return -1;
    }

    public PathCustomer PeekNextCustomer()
    {
        if (npcs.Count > 0)
            return npcs.Peek();
        return null;
    }

    public void ForceRemove(PathCustomer npc)
    {
        RemoveFromQueue(npc);
    }

    public void ClearQueue()
    {
        var currentNpcs = npcs.ToArray(); 
        npcs.Clear();

        foreach (var n in npcs)
        {
            n.OnEndDay();
        }
    }
}