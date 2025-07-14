using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    public ShopInventory shopInventory;
    public RecipeController recipeController;
    public LemonadeStandTierManager lemonadeStandTierManager;
    public UpgradeController upgradeController;
    public StatsController statsController;
    public NeonLights neonLights;

    public Toggle fpsToggle;
    public Toggle neonToggle;

    private SaveData saveData = new SaveData();
    private string filePath;

    void Start()
    {
        StartCoroutine(InitializeSaveSystem());
    }

    private IEnumerator InitializeSaveSystem()
    {
        filePath = Path.Combine(Application.persistentDataPath, "saveFile.json");

        if (File.Exists(filePath))
        {
            Load();
        }
        else
        {
            yield return SaveCoroutine(); // Wait for save to finish
            Load();
        }
    }

    private void GetShopInventoryData()
    {
        saveData.shopInventoryData.lemons = shopInventory.lemons;
        saveData.shopInventoryData.sugar = shopInventory.sugar;
        saveData.shopInventoryData.ice = shopInventory.ice;
        saveData.shopInventoryData.cups = shopInventory.cups;
        saveData.shopInventoryData.money = shopInventory.GetMoney();
    }

    private void GetLemonadeRecipeData()
    {
        saveData.lemonadeRecipeData.lemons = shopInventory.lemonsPerCup;
        saveData.lemonadeRecipeData.sugar = shopInventory.sugarPerCup;
        saveData.lemonadeRecipeData.ice = shopInventory.icePerCup;

        saveData.lemonadeRecipeData.price = shopInventory.GetLemonadePrice();
    }

    private void GetLemonadeStandTierData()
    {
        saveData.lemonadeStandTierData.tier = lemonadeStandTierManager.lemonadeStandTier;
    }

    private void GetUpgradesData()
    {
        saveData.upgradesData.juicerTier = upgradeController.currentJuicerTier;
        saveData.upgradesData.iceTier = upgradeController.currentIceTier;
        saveData.upgradesData.registerTier = upgradeController.currentRegisterTier;
        saveData.upgradesData.standTier = upgradeController.currentStandTier;
    }

    private void GetStatsData()
    {
        saveData.statsData.value = statsController.value.text;
        saveData.statsData.feedback = statsController.feedback.text;
        saveData.statsData.popularity = statsController.popularity;
        saveData.statsData.day = statsController.day;
        saveData.statsData.month = statsController.month;
        saveData.statsData.year = statsController.year;
        saveData.statsData.badPrice = statsController.badPrice;
        saveData.statsData.badRecipe = statsController.badRecipe;
    }

    private void GetGameplaySettingsData()
    {
        saveData.gameplaySettingsData.fpsCounter = fpsToggle.isOn;

        saveData.gameplaySettingsData.neonLightsCycle = neonToggle.isOn;
        saveData.gameplaySettingsData.neonLightsIntensity = neonLights.intensity;
        saveData.gameplaySettingsData.neonLightsSpeed = neonLights.scrollSpeed;
    }

    public void Save()
    {
        StartCoroutine(SaveCoroutine());
    }

    private IEnumerator SaveCoroutine()
    {
        GetShopInventoryData();
        GetStatsData();
        GetLemonadeRecipeData();
        GetLemonadeStandTierData();
        GetUpgradesData();
        GetGameplaySettingsData();

        yield return null; // wait one frame before writing

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Saved to: " + filePath);
    }

    public void Load()
    {
        string json = File.ReadAllText(filePath);
        saveData = JsonUtility.FromJson<SaveData>(json);
        Debug.Log("Loaded from: " + filePath);

        shopInventory.SetInventoryLSICM(
            saveData.shopInventoryData.lemons,
            saveData.shopInventoryData.sugar,
            saveData.shopInventoryData.ice,
            saveData.shopInventoryData.cups,
            saveData.shopInventoryData.money
        );

        statsController.LoadData(
            saveData.statsData.value,
            saveData.statsData.feedback,
            saveData.statsData.popularity,
            saveData.statsData.day,
            saveData.statsData.month,
            saveData.statsData.year,
            saveData.statsData.badPrice,
            saveData.statsData.badRecipe
        );

        recipeController.LoadRecipe(
            saveData.lemonadeRecipeData.lemons,
            saveData.lemonadeRecipeData.sugar,
            saveData.lemonadeRecipeData.ice,
            saveData.lemonadeRecipeData.price
        );

        lemonadeStandTierManager.LoadStand(
            saveData.lemonadeStandTierData.tier
        );

        upgradeController.LoadData(
            saveData.upgradesData.juicerTier,
            saveData.upgradesData.iceTier,
            saveData.upgradesData.registerTier,
            saveData.upgradesData.standTier
        );

        neonLights.LoadData(
            saveData.gameplaySettingsData.neonLightsCycle,
            saveData.gameplaySettingsData.neonLightsSpeed,
            saveData.gameplaySettingsData.neonLightsIntensity
        );

        fpsToggle.isOn = saveData.gameplaySettingsData.fpsCounter;
    }

    [System.Serializable]
    private class SaveData
    {
        public ShopInventoryData shopInventoryData = new ShopInventoryData();
        public LemonadeRecipeData lemonadeRecipeData = new LemonadeRecipeData();
        public LemonadeStandTierData lemonadeStandTierData = new LemonadeStandTierData();
        public UpgradesData upgradesData = new UpgradesData();
        public StatsData statsData = new StatsData();
        public GameplaySettingsData gameplaySettingsData = new GameplaySettingsData();
    }

    [System.Serializable]
    private class ShopInventoryData
    {
        public int lemons;
        public int sugar;
        public int ice;
        public int cups;
        public float money;
    }

    [System.Serializable]
    private class LemonadeRecipeData
    {
        public int lemons;
        public int sugar;
        public int ice;
        public float price;
    }

    [System.Serializable]
    private class LemonadeStandTierData
    {
        public int tier;
    }

    [System.Serializable]
    private class UpgradesData
    {
        public int juicerTier;
        public int iceTier;
        public int registerTier;
        public int standTier;
    }

    [System.Serializable]
    private class StatsData
    {
        public string value;
        public string feedback;

        public int year;
        public int month;
        public int day;

        public bool badRecipe;
        public bool badPrice;

        public float popularity;
    }

    [System.Serializable]
    private class GameplaySettingsData
    {
        public bool fpsCounter;
        public bool neonLightsCycle;
        public float neonLightsSpeed;
        public float neonLightsIntensity;
    }
}
