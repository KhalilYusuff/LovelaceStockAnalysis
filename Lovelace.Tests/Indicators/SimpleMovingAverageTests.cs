using Lovelace.StockAnalysis.Indicators;
using Lovelace.StockAnalysis.Models;
using Lovelace.StockAnalysis.Tests.Utilities;

namespace Lovelace.StockAnalysis.Tests.Indicators;

[TestClass]
public class SimpleMovingAverageTests
{
    [TestMethod]
    public void Calculate_ComputesSlidingWindowAverages()
    {
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 1), close: 10m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 2), close: 20m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 3), close: 30m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 4), close: 40m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 5), close: 50m),
        };
        var sma = new SimpleMovingAverage(3, point => point.Close);

        var result = sma.Calculate(data);
        var count = result.Count;
        Assert.AreEqual(3, count);
        Assert.AreEqual(20m, result[0].Value);
        Assert.AreEqual(30m, result[1].Value);
        Assert.AreEqual(40m, result[2].Value);
    }

    [TestMethod]
    public void Calculate_UsesConfiguredSelectorForAverageCalculation()
    {
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 1), volume: 1000m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 2), volume: 2000m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 3), volume: 3000m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 4), volume: 4000m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 5), volume: 5000m),
        };
        var sma = new SimpleMovingAverage(3, point => point.Volume);

        var result = sma.Calculate(data);
        var count = result.Count;

        Assert.AreEqual(3, count);
        Assert.AreEqual(2000m, result[0].Value);
        Assert.AreEqual(3000m, result[1].Value);
        Assert.AreEqual(4000m, result[2].Value);
    }

    [TestMethod]
    public void Calculate_PeriodOneReturnsEachSelectedValue()
    {
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 1), close: 100m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 2), close: 101m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 3), close: 102m),
        };
        var sma = new SimpleMovingAverage(1, point => point.Close);

        var result = sma.Calculate(data);
        var count = result.Count;

        Assert.AreEqual(3, count);
        Assert.AreEqual(100m, result[0].Value);
        Assert.AreEqual(101m, result[1].Value);
        Assert.AreEqual(102m, result[2].Value);
    }

    [TestMethod]
    public void Calculate_ReturnsSingleAverageWhenPeriodMatchesDataCount()
    {
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 1), close: 100m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 2), close: 101m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 3), close: 102m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 4), close: 103m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 5), close: 104m),
        };
        var sma = new SimpleMovingAverage(5, point => point.Close);

        var result = sma.Calculate(data);
        var count = result.Count;

        Assert.AreEqual(1, count);
        Assert.AreEqual(102m, result[0].Value);
        Assert.AreEqual(new DateTime(2024, 1, 5), result[0].Timestamp);
    }

    [TestMethod]
    public void Calculate_AssignsEachResultTimestampToTheEndOfItsWindow()
    {
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 1), close: 10m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 2), close: 20m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 3), close: 30m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 4), close: 40m),
        };
        var sma = new SimpleMovingAverage(3, point => point.Close);

        var result = sma.Calculate(data);
        var count = result.Count;

        Assert.AreEqual(2, count);
        Assert.AreEqual(new DateTime(2024, 1, 3), result[0].Timestamp);
        Assert.AreEqual(new DateTime(2024, 1, 4), result[1].Timestamp);
    }
}
