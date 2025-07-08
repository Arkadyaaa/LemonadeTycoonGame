using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ShopInventory : MonoBehaviour
{
    private int lemons;
    private int sugar;
    private int ice;
    private int cups;

    private int lemonade;
    private float lemonadePrice = 1f;

    private float money;
    public TextMeshProUGUI moneyText;

    public TextMeshProUGUI lemonsText;
    public TextMeshProUGUI sugarText;
    public TextMeshProUGUI iceText;
    public TextMeshProUGUI cupsText;

    public Image lemonadeIndicator;

    [Header("Lemonade Recipe")]
    public int lemonsPerCup = 5;
    public int sugarPerCup = 3;
    public int icePerCup = 3;
    public int cupsPerServing = 14;

    public float servings = 1f;
    private float costPerCup = 0f;

    private Coroutine craftingCoroutine;
    private bool isCrafting = false;
    public bool dayStarted = false;

    private float craftDuration = 5f;

    public StatsController statsController;

    void Start()
    {
        // RefillAll();
        // money = 10000f;

        money = 40f;

        UpdateUI();
    }

    void Update()
    {
        if (lemonade == 0)
        {
            CraftLemonade();
        }

        UpdateUI();
    }

    public void UpdateRecipe(int lemonRecipe, int sugarRecipe, int iceRecipe, int cupsRecipe, float lemonadePrice, float costPerCup)
    {
        lemonsPerCup = lemonRecipe;
        sugarPerCup = sugarRecipe;
        icePerCup = iceRecipe;
        cupsPerServing = cupsRecipe;

        this.lemonadePrice = lemonadePrice;
        this.costPerCup = costPerCup;
    }

    public void AddIngredients(int lemonsToAdd, int sugarToAdd, int iceToAdd, int cupsToAdd)
    {
        lemons += lemonsToAdd;
        sugar += sugarToAdd;
        ice += iceToAdd;
        cups += cupsToAdd;
    }

    public bool CanCraftLemonade()
    {
        return lemons >= lemonsPerCup &&
               sugar >= sugarPerCup &&
               ice >= icePerCup &&
               cups >= cupsPerServing;
    }

    public bool CraftLemonade()
    {
        if (!CanCraftLemonade() || isCrafting || !dayStarted) return false;

        craftingCoroutine = StartCoroutine(CraftLemonadeRoutine());
        return true;
    }
    
    private IEnumerator CraftLemonadeRoutine()
    {
        isCrafting = true;

        // Consume ingredients
        lemons -= lemonsPerCup;
        sugar -= sugarPerCup;
        ice -= icePerCup;
        cups -= cupsPerServing;

        float duration = craftDuration;
        float timer = 0f;

        float startFill = lemonadeIndicator.fillAmount;
        float endFill = Mathf.Clamp01((float)(lemonade + cupsPerServing * servings) / (float)(cupsPerServing * servings));

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            lemonadeIndicator.fillAmount = Mathf.Lerp(startFill, endFill, progress);
            yield return null;
        }

        lemonade += (int)(cupsPerServing * servings);
        lemonadeIndicator.fillAmount = endFill;

        isCrafting = false;
    }

    public bool SellLemonade(int amount)
    {
        if (lemonade < amount) return false;

        lemonade -= amount;
        money += lemonadePrice;

        return true;
    }

    public bool GetExpensiveState()
    {
        if (lemonadePrice <= 1.25f) return false;

        // chance to decline the order if too expensive
        float maxPrice = 5.0f;
        float minPrice = 1.25f;
        float normalized = Mathf.Clamp01((lemonadePrice - minPrice) / (maxPrice - minPrice));

        return Random.value < normalized;
    }

    public int GetLemonadeCount()
    {
        return lemonade;
    }

    public float GetMoney()
    {
        return money;
    }

    public float GetLemonadePrice()
    {
        return lemonadePrice;
    }

    public void SetLemonadePrice(float lemonadePrice)
    {
        this.lemonadePrice = lemonadePrice;
    }

    // Easy way to set the values in one method. Put -1 for the values to remain unchanged
    public void SetInventoryLSICM(int lemons, int sugar, int ice, int cups, float money)
    {
        if (lemons >= 0) this.lemons = lemons;
        if (sugar >= 0) this.sugar = sugar;
        if (ice >= 0) this.ice = ice;
        if (cups >= 0) this.cups = cups;
        if (money >= 0f) this.money = money;
    }

    public void ModifyInventoryLSICM(int lemons, int sugar, int ice, int cups, float money)
    {
        this.lemons += lemons;
        this.sugar += sugar;
        this.ice += ice;
        this.cups += cups;
        this.money += money;
    }

    void UpdateUI()
    {
        lemonsText.text = $"{lemons}";
        sugarText.text = $"{sugar}";
        iceText.text = $"{ice}";
        cupsText.text = $"{cups}";
        moneyText.text = $"$ {money:N2}";

        lemonadeIndicator.fillAmount = (float)lemonade / (float)cupsPerServing * (float)servings;
    }

    public void SupplyOnDayEnd()
    {
        statsController.LostStock(ice);
        lemonade = 0;
        ice = 0;
    }

    public void DayStartedCondition(bool condition)
    {
        dayStarted = condition;
    }

    // UPGRADES
    public void JuicerUpgrade(float craftDuration, float servings)
    {
        this.craftDuration = craftDuration;
        this.servings = servings;
    }

    //  Debug refill for testing
    private void RefillAll()
    {
        lemons = 200;
        sugar = 200;
        ice = 200;
        cups = 200;
        lemonade = 0;
    }
}
