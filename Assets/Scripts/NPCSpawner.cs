using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class NPCModelVariant
{
    public string name;
    public GameObject modelPrefab;
    public string texturesFolder;
    [HideInInspector] public Texture2D[] textures;
}

public class NPCSpawner : MonoBehaviour
{
    public GameObject[] npcPrefabs;
    public List<NPCModelVariant> npcVariants;

    public float minSpawnTime = 2f;
    public float maxSpawnTime = 5f;
    public float spawnRadius = 1f;
    public int maxGroup = 3;

    private bool enableSpawning = false;

    private Coroutine spawnCoroutine;

    [Header("NPC Behavior Settings")]
    public bool NpcDetourBeforeMidpoint = false;


    void Start()
    {
        // Load textures for each model variant
        foreach (var variant in npcVariants)
        {
            variant.textures = Resources.LoadAll<Texture2D>(variant.texturesFolder);
            Debug.Log($"Loaded {variant.textures.Length} textures for {variant.name} from folder '{variant.texturesFolder}'");
        }
    }

    public void CanSpawn(bool enableSpawning)
    {
        this.enableSpawning = enableSpawning;

        if (enableSpawning && spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnRoutine());
        }
        else if (!enableSpawning && spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            bool spawnGroup = Random.value < 0.3f;
            int groupSize = spawnGroup ? Random.Range(1, maxGroup + 1) : 1;

            for (int i = 0; i < groupSize; i++)
            {
                if (npcVariants.Count == 0) yield break;

                GameObject prefabToSpawn = npcPrefabs[Random.Range(0, npcPrefabs.Length)];

                Vector3 offset = new Vector3(
                    Random.Range(-spawnRadius, spawnRadius),
                    0,
                    Random.Range(-spawnRadius, spawnRadius)
                );

                GameObject npc = Instantiate(prefabToSpawn, transform.position + offset, Quaternion.identity);

                NPCMovement npcMovementComponent = npc.GetComponent<NPCMovement>();
                if (npcMovementComponent != null)
                {
                    npcMovementComponent.detourBeforeMidpoint = NpcDetourBeforeMidpoint;
                }

                // Pick random variant
                var variant = npcVariants[Random.Range(0, npcVariants.Count)];

                if (variant.modelPrefab == null)
                {
                    Debug.LogError($"Model prefab is null for variant: {variant.name}");
                }

                // Instantiate the model as a child of the NPC
                GameObject model = Instantiate(variant.modelPrefab, npc.transform);
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.Euler(0, 180, 0);

                // Assign random texture
                if (variant.textures != null && variant.textures.Length > 0)
                {
                    Texture2D chosenTexture = variant.textures[Random.Range(0, variant.textures.Length)];
                    Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
                    foreach (Renderer r in renderers)
                    {
                        // Make sure to use instance so we don't modify shared material
                        Material matInstance = r.material;
                        if (matInstance.HasProperty("_BaseMap"))
                            matInstance.SetTexture("_BaseMap", chosenTexture);
                        else if (matInstance.HasProperty("_MainTex")) // fallback for standard shader
                            matInstance.SetTexture("_MainTex", chosenTexture);
                    }
                }
                else
                {
                    Debug.LogWarning($"No textures found for model: {variant.name}");
                }
            }
        }
    }
}
