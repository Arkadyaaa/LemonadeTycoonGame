using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class PathCustomer : NPCPath
{
    public bool detourBeforeMidpoint = false;

    [Header("Queue")]
    public ShopPOI shopPOI;

    private UpgradeController upgradeController;
    private float waitAtShopDuration = 10f;
    private Coroutine patienceCoroutine;

    [Header("ShopOrderSystem")]
    public ShopOrderSystem shopOrderSystem;

    [Header("Dollar")]
    public GameObject floatingDollarPrefab;
    public Transform dollarSpawnPoint;

    private enum PathState { ToShop, ToMidpoint, ToEnd, WaitingAtShop, Finished }

    private Queue<PathState> pathSteps = new Queue<PathState>();
    private PathState currentState;

    private bool waiting = false;
    private bool isEndingDay = false;
    private bool served = false;

    public StatsController statsController;

    void Start()
    {
        shopOrderSystem = GetComponent<ShopOrderSystem>();
        shopPOI = GameObject.FindWithTag("shop")?.GetComponent<ShopPOI>();
        statsController = GameObject.FindWithTag("stats")?.GetComponent<StatsController>();
        dollarSpawnPoint = GameObject.FindWithTag("dollarSpawn")?.transform;
        
        upgradeController = GameObject.FindWithTag("upgrade")?.GetComponent<UpgradeController>();
        waitAtShopDuration = Random.Range(upgradeController.minQueueWaitTime, upgradeController.maxQueueWaitTime);
    }

    void Update()
    {
        if (agent == null || waiting || currentState == PathState.WaitingAtShop || currentState == PathState.Finished) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && agent.velocity.sqrMagnitude < 1f)
        {
            HandleStepComplete();
        }
    }

    protected override void SetDestination()
    {
        BuildPathQueue();
        GoToNextStep();
    }

    private void BuildPathQueue()
    {
        pathSteps.Clear();

        if (detourBeforeMidpoint)
        {
            pathSteps.Enqueue(PathState.ToShop);
            pathSteps.Enqueue(PathState.ToMidpoint);
        }
        else
        {
            pathSteps.Enqueue(PathState.ToMidpoint);
            pathSteps.Enqueue(PathState.ToShop);
        }

        pathSteps.Enqueue(PathState.ToEnd);
    }

    private void HandleStepComplete()
    {
        if (currentState == PathState.ToShop)
        {
            StartCoroutine(WaitAtShop());
        }
        else if (currentState == PathState.ToEnd)
        {
            currentState = PathState.Finished;
            shopPOI.RemoveFromQueue(this);
            npcMovement.SafelyDestroy();
        }
        else
        {
            GoToNextStep();
        }
    }

    private void GoToNextStep()
    {
        if (pathSteps.Count == 0)
        {
            currentState = PathState.Finished;
            npcMovement.SafelyDestroy();
            return;
        }

        currentState = pathSteps.Dequeue();

        switch (currentState)
        {
            case PathState.ToShop:
                agent.SetDestination(shop.position);
                break;
            case PathState.ToMidpoint:
                agent.SetDestination(midpoint.position);
                break;
            case PathState.ToEnd:
                agent.SetDestination(end.position);
                break;
        }
    }

    private IEnumerator WaitAtShop()
    {
        waiting = true;
        currentState = PathState.WaitingAtShop;

        shopPOI.RegisterInQueue(this);
        agent.SetDestination(shopPOI.GetQueuePosition(this));

        patienceCoroutine = StartCoroutine(PatienceTimer());
        yield break;
    }

    private IEnumerator PatienceTimer()
    {
        float elapsed = 0f;
        while (elapsed < waitAtShopDuration)
        {
            if (served) yield break;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Timeout: customer leaves
        CustomerFeedback("Bad");

        shopOrderSystem.PatienceExpired();
        shopPOI.RemoveFromQueue(this);
        waiting = false;
        statsController.AddSales(false);

        GoToNextStep();
    }

    public void UpdateQueuePosition(Vector3 queuePosition)
    {
        if (isEndingDay || currentState != PathState.WaitingAtShop) return;
        agent.SetDestination(queuePosition);
    }

    public void OnEndDay()
    {
        isEndingDay = true;
        StopAllCoroutines();
        waiting = false;
        currentState = PathState.ToEnd;
        agent.SetDestination(end.position);
    }

    public void OnLemonadeServed()
    {
        if (served) return;
        served = true;

        CustomerFeedback("Good");

        // Spawn the floating $ thingy above the empty game object
        if (floatingDollarPrefab != null)
        {
            Vector3 spawnPos = dollarSpawnPoint.position;
            Instantiate(floatingDollarPrefab, spawnPos, Quaternion.identity);
        }

        // Stop patience and continue path
        if (patienceCoroutine != null)
            StopCoroutine(patienceCoroutine);

        shopPOI.RemoveFromQueue(this);
        waiting = false;
        statsController.AddSales(true);
        
        GoToNextStep();
    }

    void CustomerFeedback(string feedback)
    {
        Transform canvas = transform.Find("Canvas");
        if (canvas != null)
        {
            Transform feedbackImage = canvas.Find(feedback);
            if (feedbackImage != null)
            {
                feedbackImage.gameObject.SetActive(true);
            }
        }
    }
}
