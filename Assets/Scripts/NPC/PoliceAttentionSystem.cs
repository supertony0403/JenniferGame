using UnityEngine;
using UnityEngine.Events;

public class PoliceAttentionSystem : MonoBehaviour
{
    public static PoliceAttentionSystem Instance { get; private set; }

    public float Attention { get; private set; }
    public UnityEvent<float> OnAttentionChanged;
    public UnityEvent OnMVPWarning;

    private bool _isInHome;
    private bool _warningOnCooldown;

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

    public void AddAttention(float delta)
    {
        if (_isInHome) return;
        Attention = PoliceAttentionLogic.Add(Attention, delta);
        OnAttentionChanged?.Invoke(Attention);
        if (PoliceAttentionLogic.ShouldTriggerMVPWarning(Attention) && !_warningOnCooldown)
            TriggerMVPWarning();
    }

    public void AddAttentionFromPatrol(float delta) => AddAttention(delta);

    private void TriggerMVPWarning()
    {
        _warningOnCooldown = true;
        WutMeter.Instance?.AddWut(20f);
        Attention = PoliceAttentionLogic.AfterMVPWarning(Attention);
        OnAttentionChanged?.Invoke(Attention);
        OnMVPWarning?.Invoke();
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnNewDay.AddListener(_ => _warningOnCooldown = false);
    }

    private void OnNewDay(int day)
    {
        Attention = PoliceAttentionLogic.ApplyDailyDecay(Attention);
        OnAttentionChanged?.Invoke(Attention);
    }

    public void SetInHome(bool inHome) => _isInHome = inHome;
}
