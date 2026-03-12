using UnityEngine;

public static class EconomyLogic
{
    public static float GetQualityMultiplier(string quality) => quality switch
    {
        "low"     => 0.7f,
        "medium"  => 1.0f,
        "high"    => 1.3f,
        "premium" => 1.6f,
        _ => 1.0f
    };

    public static int GetRequiredUnits(string size) => size switch
    {
        "small"  => 1,
        "medium" => 3,
        "large"  => 6,
        _ => 1
    };

    public static float CalculatePackagePrice(string size, string quality)
    {
        float basePrice = size switch
        {
            "small"  => Random.Range(80f, 120f),
            "medium" => Random.Range(220f, 350f),
            "large"  => Random.Range(400f, 650f),
            _ => 80f
        };
        return basePrice * GetQualityMultiplier(quality);
    }

    public static float GetCustomerBudget(string packageSize) => packageSize switch
    {
        "small"  => 150f,
        "medium" => 400f,
        "large"  => 700f,
        _ => 150f
    };
}
