public class PlantLogic
{
    public string PlantType { get; }
    public int PlantedDay { get; }
    public int GrowthDays { get; }
    public int MissedWaterDays { get; private set; }
    public bool IsAlive { get; private set; } = true;
    public bool WateredToday { get; private set; }

    public PlantLogic(string plantType, int plantedDay, int growthDays)
    {
        PlantType = plantType;
        PlantedDay = plantedDay;
        GrowthDays = growthDays;
    }

    public bool IsReadyToHarvest(int currentDay)
    {
        if (!IsAlive) return false;
        return currentDay >= PlantedDay + GrowthDays + MissedWaterDays;
    }

    public void Water() { WateredToday = true; MissedWaterDays = 0; }

    public void MissWatering()
    {
        MissedWaterDays++;
        if (MissedWaterDays >= 2) IsAlive = false;
    }

    public void OnNewDay() => WateredToday = false;
}
