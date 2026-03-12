using UnityEngine;

public class PackagingStation : MonoBehaviour
{
    public static PackagingStation Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public bool Pack(string size, string quality)
    {
        int required = EconomyLogic.GetRequiredUnits(size);
        if (!InventoryManager.Instance.RemoveItem("dried", quality, required))
        {
            Debug.Log($"[Packaging] Nicht genug für {size}!");
            WutMeter.Instance?.AddWut(5f);
            return false;
        }
        InventoryManager.Instance.AddItem($"package_{size}", quality);
        return true;
    }
}
