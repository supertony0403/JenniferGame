using NUnit.Framework;

public class EconomyTests
{
    [Test]
    public void QualityMultiplier_Low_Is07() =>
        Assert.AreEqual(0.7f, EconomyLogic.GetQualityMultiplier("low"), 0.001f);

    [Test]
    public void QualityMultiplier_Premium_Is16() =>
        Assert.AreEqual(1.6f, EconomyLogic.GetQualityMultiplier("premium"), 0.001f);

    [Test]
    public void UnitsRequired_SmallPackage_Is1() =>
        Assert.AreEqual(1, EconomyLogic.GetRequiredUnits("small"));

    [Test]
    public void UnitsRequired_LargePackage_Is6() =>
        Assert.AreEqual(6, EconomyLogic.GetRequiredUnits("large"));

    [Test]
    public void CustomerBudget_Large_Is700() =>
        Assert.AreEqual(700f, EconomyLogic.GetCustomerBudget("large"), 0.01f);
}
