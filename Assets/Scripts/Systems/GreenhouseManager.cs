using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GreenhouseManager : MonoBehaviour
{
    public static GreenhouseManager Instance { get; private set; }

    [SerializeField] private int maxPlants = 1;

    public UnityEvent OnPlantDied;
    public UnityEvent OnPlantHarvested;

    private readonly List<PlantLogic> _plants = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnNewDay.AddListener(OnNewDay);
    }

    public bool PlantSeed(PlantData data)
    {
        if (_plants.Count >= maxPlants) return false;
        if (!GameManager.Instance.SpendMoney(data.cost)) return false;
        _plants.Add(new PlantLogic(data.plantName, TimeManager.Instance.CurrentDay, data.growthDays));
        return true;
    }

    public bool WaterPlant(int index)
    {
        if (index < 0 || index >= _plants.Count || !_plants[index].IsAlive) return false;
        _plants[index].Water();
        return true;
    }

    public PlantLogic HarvestPlant(int index)
    {
        if (index < 0 || index >= _plants.Count) return null;
        var plant = _plants[index];
        if (!plant.IsReadyToHarvest(TimeManager.Instance.CurrentDay)) return null;
        _plants.RemoveAt(index);
        OnPlantHarvested?.Invoke();
        return plant;
    }

    private void OnNewDay(int day)
    {
        for (int i = _plants.Count - 1; i >= 0; i--)
        {
            if (!_plants[i].WateredToday) _plants[i].MissWatering();
            if (!_plants[i].IsAlive)
            {
                _plants.RemoveAt(i);
                WutMeter.Instance?.AddWut(20f);
                OnPlantDied?.Invoke();
            }
            else _plants[i].OnNewDay();
        }
    }

    public IReadOnlyList<PlantLogic> GetPlants() => _plants.AsReadOnly();
}
