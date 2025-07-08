using UnityEngine;
using TMPro;

public class RecipeController : MonoBehaviour
{
    public TextMeshProUGUI lemonsText;
    public TextMeshProUGUI sugarText;
    public TextMeshProUGUI iceText;

    public int lemonsAmount = 0;
    public int sugarAmount = 0;
    public int iceAmount = 0;

    public TextMeshProUGUI costPerCup;
    public TextMeshProUGUI cupsPerPitcher;
    public TextMeshProUGUI profitPerCup;

    public float lemonUnitPrice = 0.2f;
    public float sugarUnitPrice = 0.2f;

    public TextMeshProUGUI priceText;
    public float lemoandePrice = 1f;

    public ShopInventory shopInventory;
    public StatsController statsController;

    private float cost;
    private int cups;
    private float profit;

    void Start()
    {
        UpdateUI();
    }

    public void IncrementLemon(int amount)
    {
        if (lemonsAmount + amount == 10 || lemonsAmount + amount < 0) return;

        lemonsAmount += amount;
        UpdateUI();
    }

    public void IncrementSugar(int amount)
    {
        if (sugarAmount + amount == 10 || sugarAmount + amount < 0) return;

        sugarAmount += amount;
        UpdateUI();
    }

    public void IncrementIce(int amount)
    {
        if (iceAmount + amount == 10 || iceAmount + amount < 0) return;

        iceAmount += amount;
        UpdateUI();
    }

    public void IncrementPrice(float amount)
    {
        if (lemoandePrice + amount > 5f || lemoandePrice + amount < 0) return;

        lemoandePrice += amount;
        UpdateUI();
    }

    public bool RecipeIsEmpty()
    {
        return lemonsAmount == 0 && iceAmount == 0 && iceAmount == 0;
    }

    public float RecipeCheck()
    {
        // Preset for now
        // In the future this will change depending on the weather
        int idealLemons = 5;
        int idealSugar = 3;
        int idealIce = 3;

        // Clamp difference to a max to avoid negative satisfaction
        float lemonScore = 1f - Mathf.Min(1f, Mathf.Abs(lemonsAmount - idealLemons) / (float)idealLemons);
        float sugarScore = 1f - Mathf.Min(1f, Mathf.Abs(sugarAmount - idealSugar) / (float)idealSugar);
        float iceScore = 1f - Mathf.Min(1f, Mathf.Abs(iceAmount - idealIce) / (float)idealIce);

        float averageScore = (lemonScore + sugarScore + iceScore) / 3f;

        float satisfactionPercentage = Mathf.Round(averageScore * 100f);
        return satisfactionPercentage;
    }

    public void UpdateUI()
    {
        lemonsText.text = $"{lemonsAmount}";
        sugarText.text = $"{sugarAmount}";
        iceText.text = $"{iceAmount}";

        priceText.text = $"${lemoandePrice:F2}";

        UpdateResult();
        shopInventory.UpdateRecipe(lemonsAmount, sugarAmount, iceAmount, cups, lemoandePrice, cost);
        statsController.UpdatePrices(lemoandePrice, profit, cost);
        statsController.RecipeState(RecipeCheck());
    }

    public void UpdateResult()
    {
        cups = CalculateCups(iceAmount);
        cost = CalculatCost();
        profit = lemoandePrice - cost;

        costPerCup.text = $"COST PER CUP: ${cost}";
        cupsPerPitcher.text = $"CUPS PER PITCHER: {cups}";
        profitPerCup.text = $"PROFIT: ${profit:F2} per cup";
    }

    int CalculateCups(int x)
    {
        if (x <= 2)
        {
            return x + 10;
        }
        else
        {
            float A = 8f;
            float B = 1.21f;

            return Mathf.FloorToInt(A * Mathf.Pow(B, x));
        }
    }

    float CalculatCost()
    {
        float totalCost = (lemonsAmount * lemonUnitPrice) +
                        (sugarAmount * sugarUnitPrice) +
                        0.01f;

        return Mathf.Round(totalCost / cups * 100f) / 100f;
    }
}
