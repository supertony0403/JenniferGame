using NUnit.Framework;

public class TimeManagerTests
{
    [Test]
    public void GetHourFromProgress_At0_Returns6() =>
        Assert.AreEqual(6f, TimeLogic.GetHourFromProgress(0f), 0.01f);

    [Test]
    public void GetHourFromProgress_At05_Returns14() =>
        Assert.AreEqual(14f, TimeLogic.GetHourFromProgress(0.5f), 0.01f);

    [Test]
    public void GetHourFromProgress_At1_Returns22() =>
        Assert.AreEqual(22f, TimeLogic.GetHourFromProgress(1f), 0.01f);

    [Test]
    public void IsNight_At22_ReturnsTrue() =>
        Assert.IsTrue(TimeLogic.IsNight(22f));

    [Test]
    public void IsNight_At14_ReturnsFalse() =>
        Assert.IsFalse(TimeLogic.IsNight(14f));

    [Test]
    public void GetTimeString_At6h30_Returns0630() =>
        Assert.AreEqual("06:30", TimeLogic.GetTimeString(6.5f));
}
