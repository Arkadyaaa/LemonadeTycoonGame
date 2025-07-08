using UnityEngine;
using System.Collections;

public class ShopOrderSystem : MonoBehaviour
{
    private ShopInventory inventory;
    private ShopPOI shopPOI;
    private PathCustomer npc;
    private UpgradeController upgradeController;
    private StatsController statsController;
    
    private Coroutine orderProcessing;

    private bool beingServed = false;
    private bool hasPatience = true;
    private bool tooExpensive = false;

    void Start()
    {
        upgradeController = GameObject.FindWithTag("upgrade")?.GetComponent<UpgradeController>();
        shopPOI = GameObject.FindWithTag("shop")?.GetComponent<ShopPOI>();
        inventory = GameObject.FindWithTag("shop")?.GetComponent<ShopInventory>();
        statsController = GameObject.FindWithTag("stats")?.GetComponent<StatsController>();
        npc = GetComponent<PathCustomer>();

        tooExpensive = inventory.GetExpensiveState();
    }

    void Update()
    {
        if (shopPOI.GetQueueIndex(npc) == 0 && !beingServed && hasPatience)
        {
            beingServed = true;
            orderProcessing = StartCoroutine(OrderProcessingLoop());
        }
    }

    IEnumerator OrderProcessingLoop()
    {
        while (hasPatience)
        {
            yield return new WaitForSeconds(upgradeController.serveInterval);

            if (ProcessOrder())
            {
                yield break;
            }
        }
    }

    public void PatienceExpired()
    {
        hasPatience = false;
        if (orderProcessing != null)
            StopCoroutine(orderProcessing);
            
        orderProcessing = null;

        if (tooExpensive)
            statsController.PriceState();
    }

    bool ProcessOrder()
    {
        if (tooExpensive) return false;

        if (!inventory.SellLemonade(1))
        {
            inventory.CraftLemonade();
            return false;
        }

        shopPOI.RemoveFromQueue(npc);
        npc.OnLemonadeServed();

        return true;
    }
}
