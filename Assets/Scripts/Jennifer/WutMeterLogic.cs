public static class WutMeterLogic
{
    public static WutState GetState(float wut)
    {
        if (wut <= 30f) return WutState.Entspannt;
        if (wut <= 60f) return WutState.Gereizt;
        if (wut <= 85f) return WutState.Wuetend;
        return WutState.Ausraster;
    }

    public static float Add(float current, float delta) =>
        UnityEngine.Mathf.Clamp(current + delta, 0f, 100f);

    public static float ApplyAusrasterReset(float current) => 70f;

    public static float ApplyIntimidationBonus(float basePrice) =>
        basePrice * 1.2f;
}
