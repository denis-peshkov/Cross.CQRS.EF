namespace Cross.CQRS.EF.Tests.Tests;

[TestFixture]
public class IsolationLevelExtensionsTests
{
    [Test]
    [Category(TestCategory.UNIT)]
    [TestCase(IsolationLevel.Serializable, System.Data.IsolationLevel.Serializable)]
    [TestCase(IsolationLevel.RepeatableRead, System.Data.IsolationLevel.RepeatableRead)]
    [TestCase(IsolationLevel.ReadCommitted, System.Data.IsolationLevel.ReadCommitted)]
    [TestCase(IsolationLevel.ReadUncommitted, System.Data.IsolationLevel.ReadUncommitted)]
    [TestCase(IsolationLevel.Snapshot, System.Data.IsolationLevel.Snapshot)]
    [TestCase(IsolationLevel.Chaos, System.Data.IsolationLevel.Chaos)]
    [TestCase(IsolationLevel.Unspecified, System.Data.IsolationLevel.Unspecified)]
    public void GivenTxIsolation_WhenToDataIsolation_ThenMapsToAdoLevel(
        IsolationLevel txLevel,
        System.Data.IsolationLevel expected)
    {
        txLevel.ToDataIsolation().Should().Be(expected);
    }

    [Test]
    [Category(TestCategory.UNIT)]
    [TestCase(System.Data.IsolationLevel.Serializable, IsolationLevel.Serializable)]
    [TestCase(System.Data.IsolationLevel.RepeatableRead, IsolationLevel.RepeatableRead)]
    [TestCase(System.Data.IsolationLevel.ReadCommitted, IsolationLevel.ReadCommitted)]
    [TestCase(System.Data.IsolationLevel.ReadUncommitted, IsolationLevel.ReadUncommitted)]
    [TestCase(System.Data.IsolationLevel.Snapshot, IsolationLevel.Snapshot)]
    [TestCase(System.Data.IsolationLevel.Chaos, IsolationLevel.Chaos)]
    [TestCase(System.Data.IsolationLevel.Unspecified, IsolationLevel.Unspecified)]
    public void GivenAdoIsolation_WhenToTxIsolation_ThenMapsToTransactionsLevel(
        System.Data.IsolationLevel dataLevel,
        IsolationLevel expected)
    {
        dataLevel.ToTxIsolation().Should().Be(expected);
    }
}
