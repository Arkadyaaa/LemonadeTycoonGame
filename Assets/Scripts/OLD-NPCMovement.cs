#if false 
// DOGSHIT CODE DO NOT USE
// THIS IS AN OLD SCRIPT FOR BACKUP

using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCMovement : MonoBehaviour
{
    [Header("Point of Interest")]
    public Transform midpoint;
    public Transform end;
    public Transform shop;
    public NPCPresets npcPresets;
    public int spawnPreset = 1;

    private Vector3[] midpointShopEnd;
    private Vector3[] shopMidpointEnd;
    private Vector3[] midpointEnd;
    private Vector3[] path;

    public bool shouldDetour = false;

    [Header("Dollar")]
    public GameObject floatingDollarPrefab;
    public Transform dollarSpawnPoint;

    [Header("Optional Detour Settings")]
    [Range(0, 100)] public float detourChancePercent = 30f;
    public bool detourBeforeMidpoint = true;
    public float waitTimeAtDetour = 10f;

    [Header("Patience Timer")]
    public float minQueueWaitTime = 10f;
    public float maxQueueWaitTime = 20f;

    private bool isWaiting = false;
    private int currentIndex = 0;
    private NavMeshAgent agent;

    private ShopPOI shopPOI;
    public int queueIndex;
    private Vector3 queuePos;

    private bool hasBeenServed = false;

    // Force delete incase the NPC doesnt get deleted on endpoint
    private float lifetimeTimer = 0f;
    private float maxLifetime = 90f;

    void Start()
    {
        npcPresets = GameObject.FindWithTag("npcPresets")?.GetComponent<NPCPresets>();
        SetPOIs();

        agent = GetComponent<NavMeshAgent>();
        shopPOI = shop.GetComponent<ShopPOI>();
        queuePos = shop.position;

        shouldDetour = Random.value < detourChancePercent / 100f;

        midpointEnd = new Vector3[] { midpoint.position, end.position };
        midpointShopEnd = new Vector3[] { midpoint.position, queuePos, end.position };
        shopMidpointEnd = new Vector3[] { queuePos, midpoint.position, end.position };

        if (!shouldDetour)
        {
            path = midpointEnd;
        }
        else if (detourBeforeMidpoint)
        {
            queueIndex = shopPOI.RegisterInQueue(this);
            queuePos = shopPOI.GetQueuePosition(queueIndex);
            shopMidpointEnd[0] = queuePos;
            path = shopMidpointEnd;

            StartCoroutine(QueueTimeoutTimer());
        }
        else
        {
            path = midpointShopEnd;
        }

        agent.SetDestination(path[0]);
    }

    void Update()
    {
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer >= maxLifetime)
        {
            SafelyDestroy();
            return;
        }

        if (currentIndex >= path.Length) return; // Prevent going out of bounds

        if (!isWaiting && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }

    public void SetPOIs()
    {
        POIPreset preset = npcPresets.GetPreset(spawnPreset);

        this.midpoint = preset.midpoint;
        this.end = preset.end;
        this.shop = preset.shop;
    }

    void GoToNextPoint()
    {
        if (path[currentIndex] == queuePos)
        {
            agent.SetDestination(queuePos); // just in case

            StartCoroutine(WaitToReachPositionThen(() =>
            {
                // Only wait if this NPC is first in line
                StartCoroutine(WaitUntilFirstInLineThen(() =>
                {
                    StartCoroutine(WaitAtShopThen(() =>
                    {
                        currentIndex++; // move past shop
                        if (currentIndex < path.Length)
                        {
                            shopPOI.RemoveFromQueue(this);
                            agent.SetDestination(path[currentIndex]);
                        }
                        isWaiting = false;
                    }));
                }));
            }));

            isWaiting = true;
            return;
        }

        // Regular movement
        currentIndex++;
        if (currentIndex >= path.Length)
        {
            SafelyDestroy();
            return;
        }

        if (shouldDetour && !detourBeforeMidpoint && path[currentIndex] == queuePos)
        {
            queueIndex = shopPOI.RegisterInQueue(this);
            queuePos = shopPOI.GetQueuePosition(queueIndex);

            path[currentIndex] = queuePos;

            StartCoroutine(QueueTimeoutTimer());
        }

        agent.SetDestination(path[currentIndex]);
    }

    public void UpdateQueuePosition(Vector3 position)
    {
        agent.SetDestination(position);
    }

    IEnumerator WaitAtShopThen(System.Action continueCallback)
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(waitTimeAtDetour);
        agent.isStopped = false;
        continueCallback?.Invoke();
    }

    IEnumerator WaitToReachPositionThen(System.Action callback)
    {
        while (agent.pathPending || agent.remainingDistance > 0.5f)
        {
            yield return null;
        }
        callback?.Invoke();
    }

    IEnumerator WaitUntilFirstInLineThen(System.Action callback)
    {
        while (queueIndex >= 0)
        {
            // Refresh queueIndex in case it changed after others left
            queueIndex = shopPOI.GetQueueIndex(this);
            yield return null;
        }

        callback?.Invoke();
    }

    IEnumerator QueueTimeoutTimer()
    {
        float timeout = Random.Range(minQueueWaitTime, maxQueueWaitTime);
        yield return new WaitForSeconds(timeout);

        // Refresh queueIndex
        queueIndex = shopPOI.GetQueueIndex(this);

        if (queueIndex >= 0)
        {
            // NPC leaves the queue before being served
            shopPOI.RemoveFromQueue(this);

            // Bad Feedback
            Transform canvas = transform.Find("Canvas");
            if (canvas != null)
            {
                Transform badImage = canvas.Find("Bad");
                if (badImage != null)
                {
                    badImage.gameObject.SetActive(true);
                }
            }

            // Skip queue and go directly to next destination
            currentIndex++;
            if (currentIndex < path.Length)
            {
                agent.SetDestination(path[currentIndex]);
            }
            else
            {
                SafelyDestroy();
            }

            isWaiting = false;
        }
    }

    public void OnLemonadeServed()
    {
        if (hasBeenServed) return;
        hasBeenServed = true;

        // Good feedback
        Transform canvas = transform.Find("Canvas");
        if (canvas != null)
        {
            Transform goodImage = canvas.Find("Good");
            if (goodImage != null)
            {
                goodImage.gameObject.SetActive(true);
            }
        }

        // Spawn the floating $ thingy above the empty game object
        if (floatingDollarPrefab != null)
        {
            Vector3 spawnPos = dollarSpawnPoint.position;
            Instantiate(floatingDollarPrefab, spawnPos, Quaternion.identity);
        }

        StopAllCoroutines(); // cancel all waits

        currentIndex++;
        if (currentIndex < path.Length)
        {
            shopPOI.RemoveFromQueue(this);
            agent.isStopped = false;
            agent.SetDestination(path[currentIndex]);
        }
        else
        {
            SafelyDestroy();
        }

        isWaiting = false;
    }

    private void SafelyDestroy()
    {
        if (this != null)
        {
            Destroy(gameObject);
        }
    }
}

#endif