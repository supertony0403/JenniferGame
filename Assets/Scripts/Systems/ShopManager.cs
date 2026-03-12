using UnityEngine;
using UnityEngine.Events;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    public UnityEvent<CustomerNPC> OnCustomerInteractionStart;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartCustomerInteraction(CustomerNPC customer)
    {
        OnCustomerInteractionStart?.Invoke(customer);
        ShopUI.Instance?.ShowShopUI(customer);
    }
}
