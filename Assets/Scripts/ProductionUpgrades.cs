using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class UpgradeTier
{
    public Button buyButton;
    public TextMeshProUGUI priceText;
    public UpgradeProgressColor progress;
    public float price;
}

public class ProductionUpgrades : MonoBehaviour
{
    public UpgradeTier[] tiers;

    public ShopInventory shopInventory;

    public NotificationAnimation successNotification;
    public NotificationAnimation failNotification;

    void Start()
    {
        foreach (var tier in tiers)
        {
            tier.priceText.text = $"$ {tier.price}";
        }

        UpdateTierLocks();
    }

    public void BuyUpgrade(int tierIndex)
    {
        if (tierIndex < 0 || tierIndex >= tiers.Length) return;

        var tier = tiers[tierIndex];
        if (shopInventory.GetMoney() < tier.price)
        {
            failNotification.ShowNotification("Not enough money");
            return;
        }

        tier.progress.BuyUpgradeColor();
        tier.buyButton.interactable = false;
        shopInventory.ModifyInventoryLSICM(0, 0, 0, 0, -tier.price);

        successNotification.ShowNotification("Upgrade successfully bought");
        UpdateTierLocks();
    }

    void UpdateTierLocks()
    {
        for (int i = 0; i < tiers.Length; i++)
        {
            var tier = tiers[i];
            bool previousBought = (i == 0) || tiers[i - 1].progress.isBought;

            tier.buyButton.interactable = previousBought && !tier.progress.isBought;
            tier.progress.isDisabled = !previousBought;
            tier.progress.ApplyColor();
        }
    }
}
