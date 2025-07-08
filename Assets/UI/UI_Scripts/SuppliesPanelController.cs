using UnityEngine;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class CartItem
{
    public string product;
    public int quantity;
    public float totalPrice;

    public CartItem(string product, int quantity, float totalPrice)
    {
        this.product = product;
        this.quantity = quantity;
        this.totalPrice = totalPrice;
    }
}

[System.Serializable]
public struct Preset
{
    public int quantity;
    public float totalPrice;

    public Preset(int quantity, float totalPrice)
    {
        this.quantity = quantity;
        this.totalPrice = totalPrice;
    }
}

public class SuppliesPanelController : MonoBehaviour
{
    int maxItems = 11;
    string removeIcon = "✕";

    public TextMeshProUGUI itemText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI removeText;
    public TextMeshProUGUI totalText;

    public ShopInventory shopInventory;

    private float cartTotal = 0f;

    private List<CartItem> cart = new List<CartItem>();

    [Header("Notification")]
    public GameObject successPanel;
    public GameObject failPanel;

    public TMP_Text successText;
    public TMP_Text failText;

    public void AddLemonToCart(int presetId)
    {
        Preset preset = lemonPresets[presetId];
        AddToCart("lemons", preset.quantity, preset.totalPrice);
    }

    public void AddSugarToCart(int presetId)
    {
        Preset preset = sugarPresets[presetId];
        AddToCart("sugar", preset.quantity, preset.totalPrice);
    }

    public void AddIceToCart(int presetId)
    {
        Preset preset =icePresets[presetId];
        AddToCart("ice", preset.quantity, preset.totalPrice);
    }

    public void AddCupsToCart(int presetId)
    {
        Preset preset = cupsPresets[presetId];
        AddToCart("cups", preset.quantity, preset.totalPrice);
    }

    private void AddToCart(string product, int qty, float price)
    {
        if (cart.Count >= maxItems)
        {
            failPanel.SetActive(true);
            failText.text = "Cart is already full";
            return;
        }
        
        cart.Add(new CartItem(product, qty, price));
        UpdateCartUI();
    }

    public void RemoveFromCart(int index)
    {
        if (index >= 0 && index < cart.Count)
        {
            cart.RemoveAt(index);
            UpdateCartUI();
        }
    }

    void UpdateCartUI()
    {
        itemText.text = "";
        priceText.text = "";
        removeText.text = "";
        cartTotal = 0f;

        for (int i = 0; i < cart.Count; i++)
        {
            var item = cart[i];
            itemText.text += $"{item.quantity} x {item.product}\n";
            priceText.text += $"${item.totalPrice:F2}\n";
            removeText.text += $"<link={i}>{removeIcon}</link>\n";

            cartTotal += item.totalPrice;
        }

        totalText.text = $"Total: ${cartTotal:F2}";
    }

    public void PurchaseCart()
    {
        if (cart.Count == 0)
        {
            failPanel.SetActive(true);
            failText.text = "Cart is empty";
            return;
        }

        if (shopInventory.GetMoney() < cartTotal)
        {
            failPanel.SetActive(true);
            failText.text = "Not enough money";
            return;
        }

        int lemons = 0,
        sugar = 0,
        ice = 0,
        cups = 0;

        foreach (CartItem item in cart)
        {
            switch (item.product)
            {
                case "lemons":
                    lemons += item.quantity;
                    break;
                case "sugar":
                    sugar += item.quantity;
                    break;
                case "ice":
                    ice += item.quantity;
                    break;
                case "cups":
                    cups += item.quantity;
                    break;

            }
        }

        shopInventory.ModifyInventoryLSICM(lemons, sugar, ice, cups, -cartTotal);
        
        successPanel.SetActive(true);
        successText.text = "Items successfully purchased";

        cart.Clear();
        UpdateCartUI();
    }


    //Preset qty and prices
    private Dictionary<int, Preset> lemonPresets = new Dictionary<int, Preset>()
    {
        { 1, new Preset(12, 4.80f) },
        { 2, new Preset(24, 7.20f) },
        { 3, new Preset(48, 9.60f) }
    };

    private Dictionary<int, Preset> sugarPresets = new Dictionary<int, Preset>()
    {
        { 1, new Preset(12, 4.80f) },
        { 2, new Preset(24, 7.20f) },
        { 3, new Preset(48, 9.60f) }
    };

    private Dictionary<int, Preset> icePresets = new Dictionary<int, Preset>()
    {
        { 1, new Preset(50, 1.00f) },
        { 2, new Preset(150, 2.25f) },
        { 3, new Preset(300, 4.00f) }
    };

    private Dictionary<int, Preset> cupsPresets = new Dictionary<int, Preset>()
    {
        { 1, new Preset(75, 1.00f) },
        { 2, new Preset(225, 2.35f) },
        { 3, new Preset(400, 3.75f) }
    };
}
