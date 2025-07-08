using UnityEngine;
using TMPro;

public class StatsController : MonoBehaviour
{
    public GameObject stats_panel;
    public OpenNewPanel tab_visibility;

    public GameObject yesterday_panel;
    public GameObject selected_icon;

    public TextMeshProUGUI value;

    private int cupsSold;
    private float revenue;
    private float stock_used;
    private float stock_lost;

    private float gross_profit;
    private float gross_margin;

    private float marketing = 0f;
    private float earnings;

    private float lemonadePrice;
    private float profitPerCup;
    private float costPerCup;

    public TextMeshProUGUI feedback;

    private float satisfactionTotal;

    private int missedSales = 0;
    private bool badRecipe = false;
    private bool badPrice = false;
    private int priceFeedback;

    public TextMeshProUGUI date;
    private int year = 0;
    private int month = 0;
    private int day = 0;

    public void UpdatePrices(float lemonadePrice, float profitPerCup, float costPerCup)
    {
        this.lemonadePrice = lemonadePrice;
        this.profitPerCup = profitPerCup;
        this.costPerCup = costPerCup;
    }

    public void RecipeState(float satisfaction)
    {
        // Goofy ahh randomizer to fuck with people experimenting with the recipe
        satisfactionTotal = Mathf.Clamp(satisfaction + Random.Range(-10f, 10f), 0f, 100f);

        // Use the non random satisfaction for consistent
        badRecipe = satisfaction < 55f;
    }

    public void PriceState()
    {
        priceFeedback++;
    }

    public void AddSales(bool served)
    {
        if (!served)
        {
            missedSales++;
            return;
        }

        cupsSold++;
    }

    public void SetMarketing(float marketing)
    {
        this.marketing = marketing;
    }

    public void LostStock(int ice)
    {
        stock_lost += ice * 0.02f;
    }

    public void OnEndDay()
    {
        UpdateDate();

        CalculateDayStats();
        CalculateCustomerFeedback();

        ResetValues();

        stats_panel.SetActive(true);
        tab_visibility.CloseOthersExcept(yesterday_panel);
        tab_visibility.SelectedImage(selected_icon);
    }

    void CalculateDayStats()
    {
        revenue = cupsSold * lemonadePrice;
        stock_used = cupsSold * costPerCup;
        gross_profit = revenue - stock_used - stock_lost;

        if (revenue > 0)
            gross_margin = (gross_profit / revenue) * 100f;
        else
            gross_margin = 0f;

        earnings = gross_profit - marketing;

        value.text = "";

        value.text += $"{cupsSold} cups   ${revenue:F2}\n";
        value.text += $"${stock_used:F2}\n";
        value.text += $"${stock_lost:F2}\n\n";

        value.text += $"${gross_profit:F2}\n";
        value.text += $"{gross_margin:F2}%\n\n";

        value.text += $"${marketing:F2}\n\n";
        value.text += $"${earnings:F2}\n";
    }

    void CalculateCustomerFeedback()
    {
        badPrice = priceFeedback > (cupsSold + missedSales) / 2;

        feedback.text = "";
        feedback.text += $"Customer Satisfaction: {satisfactionTotal:F2}%\n\n";
        feedback.text += $"You missed {missedSales} sales\n\n";
        if (badRecipe) feedback.text += $"Customers complained about your recipe\nn";
        if (badPrice) feedback.text += $"Customers complained about your price";
    }

    void UpdateDate()
    {
        day++;

        if (day > 30)
        {
            day = 1;
            month++;

            if (month > 12)
            {
                month = 1;
                year++;
            }
        }

        date.text = $"Year {year}   |   Month {month}   |   Day  {day}";
    }

    void ResetValues()
    {
        cupsSold = 0;
        revenue = 0f;
        stock_used = 0f;
        stock_lost = 0f;

        gross_profit = 0f;
        gross_margin = 0f;

        marketing = 0f;
        earnings = 0f;

        missedSales = 0;
        satisfactionTotal = 0;
        satisfactionTotal = 0f;
    }
}