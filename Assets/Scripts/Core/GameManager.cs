using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float Money { get; private set; }
    public float TotalEarned { get; private set; }
    public int ProgressionLevel { get; private set; } = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() => LoadGame();

    public void AddMoney(float amount)
    {
        Money += amount;
        if (amount > 0) TotalEarned += amount;
        CheckProgression();
    }

    public bool SpendMoney(float amount)
    {
        if (Money < amount) return false;
        Money -= amount;
        return true;
    }

    private void CheckProgression()
    {
        if (ProgressionLevel == 1 && TotalEarned >= 1500f) UnlockLevel(2);
        else if (ProgressionLevel == 2 && TotalEarned >= 8000f) UnlockLevel(3);
    }

    private void UnlockLevel(int level)
    {
        ProgressionLevel = level;
        Debug.Log($"[GameManager] Level {level} freigeschaltet!");
    }

    public void SaveGame()
    {
        var data = new SaveData
        {
            money = Money,
            totalEarned = TotalEarned,
            progressionLevel = ProgressionLevel,
            currentDay = TimeManager.Instance?.CurrentDay ?? 1,
            currentHour = TimeManager.Instance?.CurrentHour ?? 6f,
            wutLevel = WutMeter.Instance?.WutLevel ?? 0f,
            policeAttention = PoliceAttentionSystem.Instance?.Attention ?? 0f,
        };

        var jennifer = GameObject.FindWithTag("Player");
        if (jennifer != null)
        {
            data.playerX = jennifer.transform.position.x;
            data.playerY = jennifer.transform.position.y;
            data.playerZ = jennifer.transform.position.z;
        }

        if (InventoryManager.Instance != null)
            foreach (var item in InventoryManager.Instance.GetItems())
                data.inventory.Add(new InventoryItemSaveData
                    { itemType = item.itemType, quality = item.quality, quantity = item.quantity });

        SaveSystem.Save(data);
        Debug.Log("[GameManager] Spiel gespeichert.");
    }

    public void LoadGame()
    {
        var data = SaveSystem.Load();
        Money = data.money;
        TotalEarned = data.totalEarned;
        ProgressionLevel = data.progressionLevel;
        TimeManager.Instance?.SetTime(data.currentHour, data.currentDay, data.dayTimer);

        if (InventoryManager.Instance != null)
            foreach (var item in data.inventory)
                InventoryManager.Instance.AddItem(item.itemType, item.quality, item.quantity);
    }
}
