using NUnit.Framework;

public class WutMeterTests
{
    [Test] public void WutState_At0_IsEntspannt() => Assert.AreEqual(WutState.Entspannt, WutMeterLogic.GetState(0f));
    [Test] public void WutState_At30_IsEntspannt() => Assert.AreEqual(WutState.Entspannt, WutMeterLogic.GetState(30f));
    [Test] public void WutState_At31_IsGereizt() => Assert.AreEqual(WutState.Gereizt, WutMeterLogic.GetState(31f));
    [Test] public void WutState_At61_IsWuetend() => Assert.AreEqual(WutState.Wuetend, WutMeterLogic.GetState(61f));
    [Test] public void WutState_At86_IsAusraster() => Assert.AreEqual(WutState.Ausraster, WutMeterLogic.GetState(86f));

    [Test]
    public void Wut_NeverExceeds100()
    {
        float wut = WutMeterLogic.Add(95f, 20f);
        Assert.AreEqual(100f, wut);
    }

    [Test]
    public void Wut_NeverBelow0()
    {
        float wut = WutMeterLogic.Add(5f, -20f);
        Assert.AreEqual(0f, wut);
    }

    [Test]
    public void AfterAusraster_WutDropsTo70() =>
        Assert.AreEqual(70f, WutMeterLogic.ApplyAusrasterReset(100f));

    [Test]
    public void IntimidationBonus_Is20Percent() =>
        Assert.AreEqual(120f, WutMeterLogic.ApplyIntimidationBonus(100f), 0.01f);
}
