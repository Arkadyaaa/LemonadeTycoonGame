using UnityEngine;

public class LemonadeStandTierManager : MonoBehaviour
{
    public int lemonadeStandTier = 0;
    public ShopInventory shopInventory;

    public GameObject[] lemonadeStands;

    [Header("Notification")]
    public NotificationAnimation successPanel;
    public NotificationAnimation failPanel;

    void OnValidate()
    {
        UpdateStand();
    }

    public void UpgradeStand(float price)
    {
        if (shopInventory.GetMoney() < price)
        {
            failPanel.ShowNotification("Not enough money");
        }

        lemonadeStandTier++;
        shopInventory.ModifyInventoryLSICM(0, 0, 0, 0, price);

        UpdateStand();
        successPanel.ShowNotification("Upgrade successful");
    }

    public void UpdateStand()
    {
        for (int i = 0; i < lemonadeStands.Length; i++)
        {
            lemonadeStands[i].SetActive(i == lemonadeStandTier);
        }
    }
}
