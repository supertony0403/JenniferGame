using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Time Settings")]
    [SerializeField] private float realSecondsPerGameDay = 1200f;

    public float CurrentHour { get; private set; }
    public int CurrentDay { get; private set; } = 1;
    public float DayProgress { get; private set; }

    public UnityEvent<int> OnNewDay;
    public UnityEvent<float> OnHourChanged;

    private float _timer;
    private float _lastHour;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CurrentHour = TimeLogic.StartHour;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        DayProgress = _timer / realSecondsPerGameDay;

        if (DayProgress >= 1f)
        {
            _timer = 0f;
            DayProgress = 0f;
            CurrentDay++;
            OnNewDay?.Invoke(CurrentDay);
        }

        CurrentHour = TimeLogic.GetHourFromProgress(DayProgress);

        if (Mathf.Floor(CurrentHour) != Mathf.Floor(_lastHour))
            OnHourChanged?.Invoke(CurrentHour);
        _lastHour = CurrentHour;
    }

    public bool IsNight() => TimeLogic.IsNight(CurrentHour);
    public string GetTimeString() => TimeLogic.GetTimeString(CurrentHour);

    public void SetTime(float hour, int day, float timer)
    {
        CurrentHour = hour;
        CurrentDay = day;
        _timer = timer;
    }
}
