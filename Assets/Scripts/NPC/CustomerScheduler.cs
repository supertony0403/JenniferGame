using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerScheduler : MonoBehaviour
{
    public static CustomerScheduler Instance { get; private set; }

    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform shopDoorSpawnPoint;
    [SerializeField] private int minDayCustomers = 3;
    [SerializeField] private int maxDayCustomers = 5;

    private readonly List<CustomerNPC> _activeCustomers = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnNewDay.AddListener(_ => StartCoroutine(SpawnCustomersForDay()));
            TimeManager.Instance.OnHourChanged.AddListener(OnHourChanged);
        }
    }

    private IEnumerator SpawnCustomersForDay()
    {
        int count = Random.Range(minDayCustomers, maxDayCustomers + 1);
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(Random.Range(60f, 300f));
            SpawnCustomer();
        }
    }

    private void OnHourChanged(float hour)
    {
        if (hour >= 20f && hour < 21f)
            StartCoroutine(SpawnEveningCustomers());
    }

    private IEnumerator SpawnEveningCustomers()
    {
        int count = Random.Range(1, 3);
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(Random.Range(30f, 120f));
            SpawnCustomer();
        }
    }

    private void SpawnCustomer()
    {
        if (customerPrefab == null || shopDoorSpawnPoint == null) return;
        var go = Instantiate(customerPrefab, shopDoorSpawnPoint.position, Quaternion.identity);
        var npc = go.GetComponent<CustomerNPC>();
        if (npc == null) return;
        npc.CustomerId = $"customer_{System.Guid.NewGuid():N}";
        _activeCustomers.Add(npc);
    }

    public void NotifyJenniferInShop()
    {
        foreach (var c in _activeCustomers)
            if (c != null && c.State == CustomerState.Waiting)
                c.OnJenniferEntersShop();
    }
}
