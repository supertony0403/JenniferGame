using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WutMeter : MonoBehaviour
{
    public static WutMeter Instance { get; private set; }

    [SerializeField] private float homeDecayRate = 2f;
    [SerializeField] private float policeRageGainRate = 5f;
    [SerializeField] private float ausrasterCooldown = 30f;

    public float WutLevel { get; private set; }
    public WutState CurrentState => WutMeterLogic.GetState(WutLevel);
    public bool CanIntimidate => CurrentState == WutState.Wuetend;

    public UnityEvent<float> OnWutChanged;
    public UnityEvent OnAusraster;

    private bool _isInHome;
    private bool _policeInSight;
    private bool _ausrasterOnCooldown;
    private float _policeTimer;
    private float _homeTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        HandlePoliceRage();
        HandleHomeDecay();
        CheckAusraster();
    }

    private void HandlePoliceRage()
    {
        if (!_policeInSight) return;
        _policeTimer += Time.deltaTime;
        if (_policeTimer >= 5f) { _policeTimer = 0f; AddWut(policeRageGainRate); }
    }

    private void HandleHomeDecay()
    {
        if (!_isInHome) return;
        _homeTimer += Time.deltaTime;
        if (_homeTimer >= 10f) { _homeTimer = 0f; AddWut(-homeDecayRate); }
    }

    private void CheckAusraster()
    {
        if (WutLevel >= 100f && !_ausrasterOnCooldown)
            StartCoroutine(TriggerAusraster());
    }

    private IEnumerator TriggerAusraster()
    {
        _ausrasterOnCooldown = true;
        OnAusraster?.Invoke();
        InventoryManager.Instance?.DestroyRandomItemOnAusraster();
        PoliceAttentionSystem.Instance?.AddAttention(15f);
        WutLevel = WutMeterLogic.ApplyAusrasterReset(WutLevel);
        OnWutChanged?.Invoke(WutLevel);
        yield return new WaitForSeconds(ausrasterCooldown);
        _ausrasterOnCooldown = false;
    }

    public void AddWut(float delta)
    {
        WutLevel = WutMeterLogic.Add(WutLevel, delta);
        OnWutChanged?.Invoke(WutLevel);
    }

    public void SetInHome(bool inHome) => _isInHome = inHome;
    public void SetPoliceInSight(bool inSight) { _policeInSight = inSight; _policeTimer = 0f; }
}
