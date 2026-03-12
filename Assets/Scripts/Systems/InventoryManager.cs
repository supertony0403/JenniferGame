using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InventoryItem
{
    public string itemType;
    public string quality;
    public int quantity;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public UnityEvent OnInventoryChanged;

    private readonly List<InventoryItem> _items = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddItem(string type, string quality, int qty = 1)
    {
        var existing = _items.Find(i => i.itemType == type && i.quality == quality);
        if (existing != null) existing.quantity += qty;
        else _items.Add(new InventoryItem { itemType = type, quality = quality, quantity = qty });
        OnInventoryChanged?.Invoke();
    }

    public bool RemoveItem(string type, string quality, int qty = 1)
    {
        var item = _items.Find(i => i.itemType == type && i.quality == quality && i.quantity >= qty);
        if (item == null) return false;
        item.quantity -= qty;
        if (item.quantity <= 0) _items.Remove(item);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public int CountItem(string type, string quality) =>
        _items.Find(i => i.itemType == type && i.quality == quality)?.quantity ?? 0;

    public IReadOnlyList<InventoryItem> GetItems() => _items.AsReadOnly();

    public void DestroyRandomItemOnAusraster()
    {
        if (_items.Count == 0) return;
        int idx = Random.Range(0, _items.Count);
        _items[idx].quantity--;
        if (_items[idx].quantity <= 0) _items.RemoveAt(idx);
        OnInventoryChanged?.Invoke();
        Debug.Log("[Inventory] Ausraster! Ein Item zerstört.");
    }
}
