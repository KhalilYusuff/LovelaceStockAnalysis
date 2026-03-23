using Lovelace.StockAnalysis.Indicators;
using Lovelace.StockAnalysis.Models;
using Lovelace.StockAnalysis.Tests.Utilities;

namespace Lovelace.StockAnalysis.Tests.Indicators;

[TestClass]
public class ExponentialMovingAverageTests
{
    [TestMethod]
    public void Constructor_NullSelector_ThrowsArgumentNullException()
    {
        Action act = () => new ExponentialMovingAverage(3, null!);
        
        Assert.ThrowsExactly<ArgumentNullException>(act);
    }

    [TestMethod]
    public void Calculate_SeedsFirstResultFromInitialSimpleMovingAverage()
    {
        var start = new DateTime(2024, 1, 1);
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(close: 10m, timestamp: start.AddDays(0)),
            StockDataPointFactory.Create(close: 11m, timestamp: start.AddDays(1)),
            StockDataPointFactory.Create(close: 12m, timestamp: start.AddDays(2)),
        };
        var ema = new ExponentialMovingAverage(3, point => point.Close);

        var result = ema.Calculate(data);
        var count = result.Count;

        Assert.AreEqual(1, count);
        Assert.AreEqual(start.AddDays(2), result[0].Timestamp);
        Assert.AreEqual(11m, result[0].Value);
    }

    [TestMethod]
    public void Calculate_AppliesRecursiveExponentialWeightingAfterSeedValue()
    {
        var start = new DateTime(2024, 1, 1);
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(close: 10m, timestamp: start.AddDays(0)),
            StockDataPointFactory.Create(close: 11m, timestamp: start.AddDays(1)),
            StockDataPointFactory.Create(close: 12m, timestamp: start.AddDays(2)),
            StockDataPointFactory.Create(close: 13m, timestamp: start.AddDays(3)),
            StockDataPointFactory.Create(close: 14m, timestamp: start.AddDays(4)),
        };
        var ema = new ExponentialMovingAverage(3, point => point.Close);

        var result = ema.Calculate(data);
        var count = result.Count;

        Assert.AreEqual(3, count);
        Assert.AreEqual(11m, result[0].Value);
        Assert.AreEqual(12m, result[1].Value);
        Assert.AreEqual(13m, result[2].Value);
    }

    [TestMethod]
    public void Calculate_UsesConfiguredSelectorForEmaComputation()
    {
        var start = new DateTime(2024, 1, 1);
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(volume: 1000m, timestamp: start.AddDays(0)),
            StockDataPointFactory.Create(volume: 2000m, timestamp: start.AddDays(1)),
            StockDataPointFactory.Create(volume: 3000m, timestamp: start.AddDays(2)),
            StockDataPointFactory.Create(volume: 4000m, timestamp: start.AddDays(3)),
            StockDataPointFactory.Create(volume: 5000m, timestamp: start.AddDays(4)),
        };
        var ema = new ExponentialMovingAverage(3, point => point.Volume);

        var result = ema.Calculate(data);
        var count = result.Count;

        Assert.AreEqual(3, count);
        Assert.AreEqual(2000m, result[0].Value);
        Assert.AreEqual(3000m, result[1].Value);
        Assert.AreEqual(4000m, result[2].Value);
    }

    [TestMethod]
    public void Calculate_ReturnsOneResultWhenPeriodMatchesDataCount()
    {
        var start = new DateTime(2024, 1, 1);
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(close: 10m, timestamp: start.AddDays(0)),
            StockDataPointFactory.Create(close: 20m, timestamp: start.AddDays(1)),
            StockDataPointFactory.Create(close: 30m, timestamp: start.AddDays(2)),
            StockDataPointFactory.Create(close: 40m, timestamp: start.AddDays(3)),
        };
        var ema = new ExponentialMovingAverage(4, point => point.Close);

        var result = ema.Calculate(data);
        var count = result.Count;

        Assert.AreEqual(1, count);
        Assert.AreEqual(25m, result[0].Value);
        Assert.AreEqual(start.AddDays(3), result[0].Timestamp);
    }

    [TestMethod]
    public void Calculate_AssignsEachResultTimestampToTheCurrentInputPoint()
    {
        var start = new DateTime(2024, 1, 1);
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(close: 10m, timestamp: start.AddDays(0)),
            StockDataPointFactory.Create(close: 11m, timestamp: start.AddDays(1)),
            StockDataPointFactory.Create(close: 12m, timestamp: start.AddDays(2)),
            StockDataPointFactory.Create(close: 13m, timestamp: start.AddDays(3)),
        };
        var ema = new ExponentialMovingAverage(3, point => point.Close);

        var result = ema.Calculate(data);
        var count = result.Count;

        Assert.AreEqual(2, count);
        Assert.AreEqual(start.AddDays(2), result[0].Timestamp);
        Assert.AreEqual(start.AddDays(3), result[1].Timestamp);
    }
}
