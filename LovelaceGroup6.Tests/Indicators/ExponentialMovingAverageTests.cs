using LovelaceGroup6.src.StockAnalysis.Indicators;
using LovelaceGroup6.StockAnalysis.Models;
using LovelaceGroup6.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LovelaceGroup6.Tests.Indicators;

[TestClass]
public class ExponentialMovingAverageTests
{
    private static List<StockDataPoint> CreateTestData(int count, decimal basePrice = 100m)
    {
        var data = new List<StockDataPoint>();
        var startDate = new DateTime(2024, 1, 1);

        for (int i = 0; i < count; i++)
        {
            var price = basePrice + i;
            data.Add(new StockDataPoint(
                timestamp: startDate.AddDays(i),
                open: price,
                high: price + 2,
                low: price - 1,
                close: price,
                volume: 1000m 
            ));
        }

        return data;
    }

    #region Constructor Tests

    [TestMethod]
    public void Constructor_ValidParameters_CreatesInstance()
    {
        var ema = new ExponentialMovingAverage(3, p => p.Close);
        Assert.IsNotNull(ema);
    }

    [TestMethod]
    public void Constructor_NullSelector_ThrowsArgumentNullException()
    {
        Action act = () => new ExponentialMovingAverage(3, null!);
        Assert.ThrowsExactly<ArgumentNullException>(act);
    }

    #endregion

    #region Calculate Method - Valid Cases

    [TestMethod]
    public void Calculate_ValidData_ReturnsExpectedCount()
    {
        var period = 3;
        var ema = new ExponentialMovingAverage(period, p => p.Close);

        var start = new DateTime(2024, 1, 1);
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(close: 10m, timestamp: start.AddDays(0)),
            StockDataPointFactory.Create(close: 11m, timestamp: start.AddDays(1)),
            StockDataPointFactory.Create(close: 12m, timestamp: start.AddDays(2)),
            StockDataPointFactory.Create(close: 13m, timestamp: start.AddDays(3)),
            StockDataPointFactory.Create(close: 14m, timestamp: start.AddDays(4)),
        };

        var result = ema.Calculate(data);

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public void Calculate_Period3_CalculatesCorrectValues()
    {
        var ema = new ExponentialMovingAverage(3, p => p.Close);

        var start = new DateTime(2024, 1, 1);
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(close: 10m, timestamp: start.AddDays(0)),
            StockDataPointFactory.Create(close: 11m, timestamp: start.AddDays(1)),
            StockDataPointFactory.Create(close: 12m, timestamp: start.AddDays(2)),
            StockDataPointFactory.Create(close: 13m, timestamp: start.AddDays(3)),
            StockDataPointFactory.Create(close: 14m, timestamp: start.AddDays(4)),
        };

        var result = ema.Calculate(data);

        Assert.AreEqual(3, result.Count);

        Assert.AreEqual(start.AddDays(2), result[0].Timestamp);
        Assert.AreEqual(11m, result[0].Value);

        Assert.AreEqual(start.AddDays(3), result[1].Timestamp);
        Assert.AreEqual(12m, result[1].Value);

        Assert.AreEqual(start.AddDays(4), result[2].Timestamp);
        Assert.AreEqual(13m, result[2].Value);
    }

    [TestMethod]
    public void Calculate_DifferentSelector_UsesSelectedProperty()
    {
        var ema = new ExponentialMovingAverage(3, p => p.Volume);

        var start = new DateTime(2024, 1, 1);
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(volume: 1000m, timestamp: start.AddDays(0)),
            StockDataPointFactory.Create(volume: 2000m, timestamp: start.AddDays(1)),
            StockDataPointFactory.Create(volume: 3000m, timestamp: start.AddDays(2)),
            StockDataPointFactory.Create(volume: 4000m, timestamp: start.AddDays(3)),
            StockDataPointFactory.Create(volume: 5000m, timestamp: start.AddDays(4)),
        };

        var result = ema.Calculate(data);

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(2000m, result[0].Value);
        Assert.AreEqual(3000m, result[1].Value);
        Assert.AreEqual(4000m, result[2].Value);
    }

    
    [TestMethod]
    public void Calculate_LargeDataset_ProcessesCorrectly()
    {
        var data = CreateTestData(count: 1000, basePrice: 100m);
        var ema = new ExponentialMovingAverage(20, p => p.Close);

        var result = ema.Calculate(data);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Count > 0);
        Assert.AreEqual(1000 - 20 + 1, result.Count);
    }

    
    [TestMethod]
    public void Calculate_ConsecutiveCalls_ReturnsSameResults()
    {
        var data = CreateTestData(count: 50, basePrice: 10m);
        var ema = new ExponentialMovingAverage(10, p => p.Close);

        var result1 = ema.Calculate(data);
        var result2 = ema.Calculate(data);

        Assert.AreEqual(result1.Count, result2.Count);

        for (int i = 0; i < result1.Count; i++)
        {
            Assert.AreEqual(result1[i].Timestamp, result2[i].Timestamp);
            Assert.AreEqual(result1[i].Value, result2[i].Value);
        }
    }

    #endregion

    #region Timestamp Tests

    [TestMethod]
    public void Calculate_TimestampsAlignWithInputDataFromPeriodMinusOne()
    {
        var ema = new ExponentialMovingAverage(3, p => p.Close);

        var start = new DateTime(2024, 1, 1);
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(close: 10m, timestamp: start.AddDays(0)),
            StockDataPointFactory.Create(close: 11m, timestamp: start.AddDays(1)),
            StockDataPointFactory.Create(close: 12m, timestamp: start.AddDays(2)),
            StockDataPointFactory.Create(close: 13m, timestamp: start.AddDays(3)),
        };

        var result = ema.Calculate(data);

        Assert.AreEqual(start.AddDays(2), result[0].Timestamp);
        Assert.AreEqual(start.AddDays(3), result[1].Timestamp);
    }

    
    [TestMethod]
    public void Calculate_ChronologicalOrder_MaintainsOrder()
    {
        var data = CreateTestData(30, 100m);
        var ema = new ExponentialMovingAverage(5, p => p.Close);

        var result = ema.Calculate(data);

        for (int i = 1; i < result.Count; i++)
        {
            Assert.IsTrue(result[i].Timestamp > result[i - 1].Timestamp);
        }
    }

    #endregion

    #region Validation Tests

    [TestMethod]
    public void Calculate_NullData_ThrowsException()
    {
        var ema = new ExponentialMovingAverage(3, p => p.Close);
        Action act = () => ema.Calculate(null!);

        Assert.ThrowsExactly<ArgumentNullException>(act);
    }

    [TestMethod]
    public void Calculate_PeriodExceedsDataCount_ThrowsException()
    {
        var ema = new ExponentialMovingAverage(10, p => p.Close);

        var start = new DateTime(2024, 1, 1);
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(close: 10m, timestamp: start.AddDays(0)),
            StockDataPointFactory.Create(close: 11m, timestamp: start.AddDays(1)),
            StockDataPointFactory.Create(close: 12m, timestamp: start.AddDays(2)),
        };

        Action act = () => ema.Calculate(data);

        Assert.ThrowsExactly<ArgumentException>(act);
    }

    #endregion
}