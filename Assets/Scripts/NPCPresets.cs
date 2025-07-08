using UnityEngine;

[System.Serializable]
public class POIPreset
{
    public Transform midpoint;
    public Transform end;
    public Transform shop;

    public POIPreset(Transform midpoint, Transform end, Transform shop)
    {
        this.midpoint = midpoint;
        this.end = end;
        this.shop = shop;
    }
}

public class NPCPresets : MonoBehaviour
{
    public Transform midpoint;
    public Transform shop;

    public Transform northEnd;
    public Transform eastEnd;
    public Transform westEnd;

    void Start()
    {
        midpoint = GameObject.FindWithTag("midpoint")?.transform;
        shop = GameObject.FindWithTag("shop")?.transform;


        northEnd = GameObject.FindWithTag("north")?.transform;
        eastEnd = GameObject.FindWithTag("east")?.transform;
        westEnd = GameObject.FindWithTag("west")?.transform;
    }

    public POIPreset GetPreset(int preset)
    {
        switch (preset)
        {
            case 1:
                return new POIPreset(midpoint, northEnd, shop);
            case 2:
                return new POIPreset(midpoint, eastEnd, shop);
            case 3:
                return new POIPreset(midpoint, westEnd, shop);
            default:
                Debug.LogWarning("Invalid preset number.");
                return new POIPreset(midpoint, northEnd, shop);
        }
    }
}
