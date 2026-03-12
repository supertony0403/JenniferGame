using System.Collections;
using UnityEngine;

public enum CustomerState { Waiting, InShop, Buying, Leaving }

public class CustomerNPC : MonoBehaviour
{
    [SerializeField] private float waitTimeout = 60f;
    [SerializeField] private string preferredSize = "small";
    [SerializeField] private bool isLoyalCustomer;

    public string CustomerId { get; set; }
    public int PurchaseCount { get; private set; }
    public CustomerState State { get; private set; } = CustomerState.Waiting;

    private float _waitTimer;

    private void Update()
    {
        if (State != CustomerState.Waiting) return;
        _waitTimer += Time.deltaTime;
        if (_waitTimer >= waitTimeout) Leave();
    }

    public void OnJenniferEntersShop()
    {
        if (State != CustomerState.Waiting) return;
        State = CustomerState.InShop;
        ShopManager.Instance?.StartCustomerInteraction(this);
    }

    public bool TryBuy(string packageType, float offeredPrice)
    {
        float budget = EconomyLogic.GetCustomerBudget(preferredSize);
        if (!isLoyalCustomer && offeredPrice > budget)
        {
            WutMeter.Instance?.AddWut(10f);
            return false;
        }

        GameManager.Instance?.AddMoney(offeredPrice);
        InventoryManager.Instance?.RemoveItem(packageType, "medium");
        WutMeter.Instance?.AddWut(-10f);
        AudioManager.Instance?.PlayPurchase();
        PurchaseCount++;
        if (PurchaseCount >= 5) isLoyalCustomer = true;
        State = CustomerState.Leaving;
        StartCoroutine(DespawnAfterDelay(1f));
        return true;
    }

    public bool TryIntimidate(float basePrice)
    {
        if (WutMeter.Instance == null || !WutMeter.Instance.CanIntimidate) return false;
        if (Random.value <= 0.7f)
        {
            TryBuy($"package_{preferredSize}", WutMeterLogic.ApplyIntimidationBonus(basePrice));
            return true;
        }
        WutMeter.Instance?.AddWut(5f);
        Leave();
        return false;
    }

    private void Leave()
    {
        State = CustomerState.Leaving;
        StartCoroutine(DespawnAfterDelay(0.5f));
    }

    private IEnumerator DespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
