using NUnit.Framework;

public class PlantSystemTests
{
    [Test]
    public void Plant_ReadyToHarvest_After3Days()
    {
        var plant = new PlantLogic("cheap", plantedDay: 1, growthDays: 3);
        Assert.IsTrue(plant.IsReadyToHarvest(currentDay: 4));
    }

    [Test]
    public void Plant_NotReady_Before3Days()
    {
        var plant = new PlantLogic("cheap", plantedDay: 1, growthDays: 3);
        Assert.IsFalse(plant.IsReadyToHarvest(currentDay: 3));
    }

    [Test]
    public void Plant_Dies_After2MissedWaterDays()
    {
        var plant = new PlantLogic("cheap", plantedDay: 1, growthDays: 3);
        plant.MissWatering();
        plant.MissWatering();
        Assert.IsFalse(plant.IsAlive);
    }

    [Test]
    public void Plant_LosesGrowthDay_After1MissedWater()
    {
        var plant = new PlantLogic("cheap", plantedDay: 1, growthDays: 3);
        plant.MissWatering();
        Assert.AreEqual(1, plant.MissedWaterDays);
        Assert.IsTrue(plant.IsAlive);
    }

    [Test]
    public void Watering_ResetsMissedDays()
    {
        var plant = new PlantLogic("cheap", plantedDay: 1, growthDays: 3);
        plant.MissWatering();
        plant.Water();
        Assert.AreEqual(0, plant.MissedWaterDays);
    }
}
