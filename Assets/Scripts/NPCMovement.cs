using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCMovement : MonoBehaviour
{
    [Header("Point of Interest")]
    public Transform midpoint;
    public Transform end;
    public Transform shop;
    public NPCPresets npcPresets;
    public int spawnPreset = 1;

    public PathNormal pathNormal;
    public PathCustomer pathCustomer;

    public bool shouldDetour = false;

    [Header("Optional Detour Settings")]
    public bool detourBeforeMidpoint = true;

    public NavMeshAgent agent;

    private ShopPOI shopPOI;
    public int queueIndex;

    private UpgradeController upgradeController;

    void Start()
    {
        npcPresets = GameObject.FindWithTag("npcPresets")?.GetComponent<NPCPresets>();
        SetPOIs();
        
        agent = GetComponent<NavMeshAgent>();
        shopPOI = shop.GetComponent<ShopPOI>();

        upgradeController = GameObject.FindWithTag("upgrade")?.GetComponent<UpgradeController>();
        shouldDetour = Random.value < upgradeController.detourChancePercent / 100f;

        // Pathing
        if (shouldDetour)
        {
            if (detourBeforeMidpoint) pathCustomer.detourBeforeMidpoint = true;

            pathCustomer.SetPath(midpoint, end, shop, this);
        }
        else
        {
            pathNormal.SetPath(midpoint, end, shop, this);
        }
    }

    void Update()
    {

    }

    public void SetPOIs()
    {
        POIPreset preset = npcPresets.GetPreset(spawnPreset);

        this.midpoint = preset.midpoint;
        this.end = preset.end;
        this.shop = preset.shop;
    }

    public void SafelyDestroy()
    {
        if (this != null)
        {
            Destroy(gameObject);
        }
    }
}