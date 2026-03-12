using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance { get; private set; }

    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI packageCountText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Slider wutSlider;
    [SerializeField] private Image wutFill;
    [SerializeField] private Slider policeSlider;
    [SerializeField] private Image policeFill;

    [Header("Wut Colors")]
    [SerializeField] private Color entspanntColor = Color.green;
    [SerializeField] private Color gereizterColor = Color.yellow;
    [SerializeField] private Color wuetendColor = new Color(1f, 0.5f, 0f);
    [SerializeField] private Color ausrasterColor = Color.red;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        WutMeter.Instance?.OnWutChanged.AddListener(UpdateWutUI);
        PoliceAttentionSystem.Instance?.OnAttentionChanged.AddListener(UpdatePoliceUI);
        PoliceAttentionSystem.Instance?.OnMVPWarning.AddListener(FlashPoliceWarning);
    }

    private void Update()
    {
        if (GameManager.Instance != null && moneyText != null)
            moneyText.text = $"$ {GameManager.Instance.Money:F0}";
        if (TimeManager.Instance != null && timeText != null)
            timeText.text = TimeManager.Instance.GetTimeString();

        int packages = 0;
        if (InventoryManager.Instance != null)
            foreach (var item in InventoryManager.Instance.GetItems())
                if (item.itemType.StartsWith("package")) packages += item.quantity;
        if (packageCountText != null) packageCountText.text = $"📦 {packages}";
    }

    private void UpdateWutUI(float wut)
    {
        if (wutSlider != null) wutSlider.value = wut / 100f;
        if (wutFill == null) return;
        wutFill.color = WutMeterLogic.GetState(wut) switch
        {
            WutState.Entspannt => entspanntColor,
            WutState.Gereizt   => gereizterColor,
            WutState.Wuetend   => wuetendColor,
            WutState.Ausraster => ausrasterColor,
            _ => entspanntColor
        };
    }

    private void UpdatePoliceUI(float attention)
    {
        if (policeSlider != null) policeSlider.value = attention / 100f;
    }

    private void FlashPoliceWarning() =>
        StartCoroutine(FlashColor(policeFill, Color.red, 0.5f));

    private IEnumerator FlashColor(Image img, Color flashColor, float duration)
    {
        if (img == null) yield break;
        Color original = img.color;
        img.color = flashColor;
        yield return new WaitForSeconds(duration);
        img.color = original;
    }
}
