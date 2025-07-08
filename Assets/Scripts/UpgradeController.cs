using UnityEngine;

public class UpgradeController : MonoBehaviour
{
    public ShopInventory shopInventory;

    private int currentJuicerTier = 0;
    private int currentIceTier = 0;
    private int currentRegisterTier = 0;
    private int currentStandTier = 0;

    public int startingIce = 0;
    public float serveInterval = 5f;
    public float detourChancePercent = 40f;
    
    public float minQueueWaitTime = 10f;
    public float maxQueueWaitTime = 20f;

    public void UpgradeJuicer(int tier)
    {
        if (tier == currentJuicerTier) return;
        currentJuicerTier++;

        switch (currentJuicerTier)
        {
            case 1:
                shopInventory.JuicerUpgrade(5f, 1.25f);
                break;
            case 2:
                shopInventory.JuicerUpgrade(2.5f, 1.25f);
                break;
            case 3:
                shopInventory.JuicerUpgrade(2.5f, 1.5f);
                break;
        }
    }

    public void UpgradeIce(int tier)
    {
        if (tier == currentIceTier) return;
        currentIceTier++;

        switch (currentIceTier)
        {
            case 1:
                startingIce = 50;
                break;
            case 2:
                startingIce = 150;
                break;
            case 3:
                startingIce = 300;
                break;
        }

        shopInventory.SetInventoryLSICM(-1, -1, startingIce, -1, -1f);
    }

    public void UpgradeRegister(int tier)
    {
        if (tier == currentRegisterTier) return;
        currentRegisterTier++;

        switch (currentRegisterTier)
        {
            case 1:
                serveInterval = 2.5f;
                break;
            case 2:
                serveInterval = 1f;
                break;
        }
    }

    public void UpgradeStand(int tier)
    {
        if (tier == currentStandTier) return;
        currentStandTier++;

        switch (currentStandTier)
        {
            case 1:
                minQueueWaitTime += 5f;
                maxQueueWaitTime += 5f;
                break;
            case 2:
                minQueueWaitTime += 2.5f;
                maxQueueWaitTime += 2.5f;
                detourChancePercent += 10f;
                break;
            case 3:
                detourChancePercent += 20f;
                break;
        }
    }
}
