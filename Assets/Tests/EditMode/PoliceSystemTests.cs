using NUnit.Framework;

public class PoliceSystemTests
{
    [Test]
    public void Attention_ClampedAt100() =>
        Assert.AreEqual(100f, PoliceAttentionLogic.Add(90f, 20f));

    [Test]
    public void Attention_NeverBelow0() =>
        Assert.AreEqual(0f, PoliceAttentionLogic.Add(5f, -20f));

    [Test]
    public void At100_TriggersMVPWarning() =>
        Assert.IsTrue(PoliceAttentionLogic.ShouldTriggerMVPWarning(100f));

    [Test]
    public void At99_NoWarning() =>
        Assert.IsFalse(PoliceAttentionLogic.ShouldTriggerMVPWarning(99f));

    [Test]
    public void DailyDecay_ReducesByFive() =>
        Assert.AreEqual(45f, PoliceAttentionLogic.ApplyDailyDecay(50f));
}
