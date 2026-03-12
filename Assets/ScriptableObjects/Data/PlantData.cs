using UnityEngine;

[CreateAssetMenu(fileName = "PlantData", menuName = "Jennifer/Plant Data")]
public class PlantData : ScriptableObject
{
    public string plantName;
    public float cost;
    public int growthDays;
    public int yieldUnits;
    public string maxQuality;
    public int dryingDays;
}
