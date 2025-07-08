using UnityEngine;
using System.Collections;

public class DayController : MonoBehaviour
{
    public NPCSpawner[] spawners;
    public GameObject startDay;
    public GameObject timer;
    public TimeController timeController;
    public ShopPOI shopPOI;
    public LightingManager lightingManager;

    [Header("Script")]
    public ShopInventory shopInventory;
    public UpgradeController upgradeController;
    public StatsController statsController;
    public RecipeController recipeController;

    [Header("Notification")]
    public NotificationAnimation successNotif;
    public NotificationAnimation failNotif;

    public InventorySlideAnimation lemonadeInventory;

    public int totalDayDuration = 300;

    void Start()
    {
        shopInventory.ModifyInventoryLSICM(0, 0, upgradeController.startingIce, 0, 0f);

        lemonadeInventory.Hide();
        lightingManager.TimeOfDay = 9f;
    }

    public void StartDay(int spawnDuration)
    {
        if (recipeController.RecipeIsEmpty())
        {
            failNotif.ShowNotification("Recipe is not set");
            return;
        }

        if (!shopInventory.CanCraftLemonade())
        {
            failNotif.ShowNotification("Not enough supplies");
            return;
        }

        shopInventory.DayStartedCondition(true);
        lemonadeInventory.Show();
        StartCoroutine(EnableSpawnersCoroutine(spawnDuration, totalDayDuration));
    }

    private IEnumerator EnableSpawnersCoroutine(int spawnDuration, int totalDayDuration)
    {
        lightingManager.TimeOfDay = 9f;
        SetState(true);

        // Enable spawning
        foreach (var spawner in spawners)
        {
            spawner.CanSpawn(true);
        }

        // Wait only for spawn duration
        yield return new WaitForSeconds(spawnDuration);

        // Disable spawning
        foreach (var spawner in spawners)
        {
            spawner.CanSpawn(false);
        }

        // Wait the remaining time to complete full day duration
        float remainingTime = Mathf.Max(0, totalDayDuration - spawnDuration);
        yield return new WaitForSeconds(remainingTime);

        SetState(false);
        EndDay();
    }

    private void SetState(bool active)
    {
        timeController.SetFastForward(false);

        lightingManager.isTimeRunning = active;
        timer.SetActive(active);
        startDay.SetActive(!active);
    }

    private void EndDay()
    {
        // This causes issues
        // DestroyNPCs();

        shopInventory.DayStartedCondition(false);
        shopInventory.SupplyOnDayEnd();
        shopInventory.ModifyInventoryLSICM(0, 0, upgradeController.startingIce, 0, 0f);
        lemonadeInventory.Hide();
        shopPOI.ClearQueue();
        statsController.OnEndDay();
    }

    private void DestroyNPCs()
    {
        GameObject[] allNPCs = GameObject.FindGameObjectsWithTag("npc");

        if (allNPCs.Length == 0) return;

        foreach (GameObject npc in allNPCs)
        {
            Destroy(npc);
        }
    }
}
