public static class PoliceAttentionLogic
{
    public static float Add(float current, float delta) =>
        UnityEngine.Mathf.Clamp(current + delta, 0f, 100f);

    public static bool ShouldTriggerMVPWarning(float attention) => attention >= 100f;

    public static float ApplyDailyDecay(float current) => Add(current, -5f);

    public static float AfterMVPWarning(float current) => 50f;
}
