using System;
using System.Collections.Generic;

[Serializable]
public class PlantSaveData
{
    public string plantType;
    public int plantedDay;
    public int lastWateredDay;
    public int missedWaterDays;
    public bool isAlive;
}

[Serializable]
public class DryingItemSaveData
{
    public string quality;
    public int startDay;
    public int durationDays;
    public int units;
}

[Serializable]
public class InventoryItemSaveData
{
    public string itemType;
    public string quality;
    public int quantity;
}

[Serializable]
public class CustomerSaveData
{
    public string customerId;
    public int purchaseCount;
    public bool isLoyalCustomer;
}

[Serializable]
public class SaveData
{
    public float money = 500f;
    public float totalEarned = 0f;
    public int currentDay = 1;
    public float currentHour = 6f;
    public float dayTimer = 0f;
    public int progressionLevel = 1;
    public float wutLevel = 0f;
    public float policeAttention = 0f;
    public bool blackMarketUnlocked = false;
    public bool employeeHired = false;
    public float playerX, playerY, playerZ;
    public List<PlantSaveData> plants = new();
    public List<DryingItemSaveData> dryingItems = new();
    public List<InventoryItemSaveData> inventory = new();
    public List<CustomerSaveData> customers = new();
}
