using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DryingItem
{
    public string quality;
    public int startDay;
    public int durationDays;
    public int units;
    public bool perfectDrying;
}

public class DryingRack : MonoBehaviour
{
    public static DryingRack Instance { get; private set; }

    private readonly List<DryingItem> _drying = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnNewDay.AddListener(CheckDryingComplete);
    }

    public bool StartDrying(string quality, int units, int durationDays)
    {
        _drying.Add(new DryingItem
        {
            quality = quality,
            startDay = TimeManager.Instance.CurrentDay,
            durationDays = durationDays,
            units = units,
            perfectDrying = true
        });
        return true;
    }

    private void CheckDryingComplete(int day)
    {
        for (int i = _drying.Count - 1; i >= 0; i--)
        {
            var item = _drying[i];
            if (day - item.startDay >= item.durationDays)
            {
                string finalQuality = item.perfectDrying && item.quality == "high"
                    ? "premium" : item.quality;
                InventoryManager.Instance?.AddItem("dried", finalQuality, item.units);
                _drying.RemoveAt(i);
                Debug.Log($"[Drying] Fertig: {item.units}x {finalQuality}");
            }
        }
    }

    public IReadOnlyList<DryingItem> GetDryingItems() => _drying.AsReadOnly();
}
