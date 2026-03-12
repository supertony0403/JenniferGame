using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    [SerializeField] private GameObject shopPanel;
    [SerializeField] private TextMeshProUGUI customerInfoText;
    [SerializeField] private Button sellSmallButton;
    [SerializeField] private Button sellMediumButton;
    [SerializeField] private Button intimidateButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject blackMarketHintPanel;

    private CustomerNPC _currentCustomer;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        shopPanel.SetActive(false);
    }

    private void Start()
    {
        sellSmallButton.onClick.AddListener(() => SellPackage("small"));
        sellMediumButton.onClick.AddListener(() => SellPackage("medium"));
        intimidateButton.onClick.AddListener(OnIntimidatePressed);
        closeButton.onClick.AddListener(CloseShop);
    }

    public void ShowShopUI(CustomerNPC customer)
    {
        _currentCustomer = customer;
        shopPanel.SetActive(true);

        int smallCount = InventoryManager.Instance?.CountItem("package_small", "medium") ?? 0;
        int mediumCount = InventoryManager.Instance?.CountItem("package_medium", "medium") ?? 0;

        if (customerInfoText != null)
            customerInfoText.text = $"Kunde wartet\nKlein: {smallCount}x  |  Mittel: {mediumCount}x";

        sellSmallButton.interactable = smallCount > 0;
        sellMediumButton.interactable = mediumCount > 0;
        intimidateButton.interactable = WutMeter.Instance?.CanIntimidate ?? false;

        bool hasPremium = (InventoryManager.Instance?.CountItem("package_large", "premium") ?? 0) > 0;
        if (blackMarketHintPanel != null)
            blackMarketHintPanel.SetActive(hasPremium && GameManager.Instance?.ProgressionLevel < 3);
    }

    private void SellPackage(string size)
    {
        if (_currentCustomer == null) return;
        float price = EconomyLogic.CalculatePackagePrice(size, "medium");
        _currentCustomer.TryBuy($"package_{size}", price);
        CloseShop();
    }

    public void OnIntimidatePressed()
    {
        if (_currentCustomer == null || !shopPanel.activeSelf) return;
        float basePrice = EconomyLogic.CalculatePackagePrice("small", "medium");
        _currentCustomer.TryIntimidate(basePrice);
        CloseShop();
    }

    private void CloseShop() => shopPanel.SetActive(false);
}
