using LovelaceGroup6.src.StockAnalysis.Indicators;
using LovelaceGroup6.StockAnalysis.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LovelaceGroup6.Tests;

[TestClass]
public class SimpleMovingAverageTests
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
                volume: 1000
            ));
        }

        return data;
    }

    #region Constructor Tests

    [TestMethod]
    public void Constructor_ValidParameters_CreatesInstance()
    {
        var sma = new SimpleMovingAverage(5, data => data.Close);

        Assert.IsNotNull(sma);
    }

    [TestMethod]
    public void Constructor_NullSelector_ThrowsArgumentNullException()
    {
        Action act = () => new SimpleMovingAverage(5, null!);

        Assert.ThrowsExactly<ArgumentNullException>(act);
    }

    #endregion

    #region Calculate Method - Valid Cases

    [TestMethod]
    public void Calculate_ValidData_ReturnsCorrectMovingAverages()
    {
        var data = CreateTestData(10, 100m);
        var sma = new SimpleMovingAverage(3, data => data.Close);

        var result = sma.Calculate(data);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Count > 0);
    }

    [TestMethod]
    public void Calculate_Period3_CalculatesCorrectValues()
    {
        var data = new List<StockDataPoint>
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 10, 12, 9, 10, 1000),
            new StockDataPoint(new DateTime(2024, 1, 2), 20, 22, 19, 20, 1000),
            new StockDataPoint(new DateTime(2024, 1, 3), 30, 32, 29, 30, 1000),
            new StockDataPoint(new DateTime(2024, 1, 4), 40, 42, 39, 40, 1000),
            new StockDataPoint(new DateTime(2024, 1, 5), 50, 52, 49, 50, 1000)
        };
        var sma = new SimpleMovingAverage(3, data => data.Close);

        var result = sma.Calculate(data);

        Assert.AreEqual(3, result.Count);
        // First SMA: (10 + 20 + 30) / 3 = 20
        Assert.AreEqual(20m, result[0].Value);
        Assert.AreEqual(new DateTime(2024, 1, 3), result[0].Timestamp);
        // Second SMA: (20 + 30 + 40) / 3 = 30
        Assert.AreEqual(30m, result[1].Value);
        Assert.AreEqual(new DateTime(2024, 1, 4), result[1].Timestamp);
        // Third SMA: (30 + 40 + 50) / 3 = 40
        Assert.AreEqual(40m, result[2].Value);
        Assert.AreEqual(new DateTime(2024, 1, 5), result[2].Timestamp);
    }

    [TestMethod]
    public void Calculate_Period2_CalculatesCorrectValues()
    {
        var data = new List<StockDataPoint>
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 10, 12, 9, 100, 1000),
            new StockDataPoint(new DateTime(2024, 1, 2), 20, 22, 19, 200, 1000),
            new StockDataPoint(new DateTime(2024, 1, 3), 30, 32, 29, 300, 1000),
            new StockDataPoint(new DateTime(2024, 1, 4), 40, 42, 39, 400, 1000)
        };
        var sma = new SimpleMovingAverage(2, data => data.Close);

        var result = sma.Calculate(data);

        Assert.AreEqual(3, result.Count);
        // First: (100 + 200) / 2 = 150
        Assert.AreEqual(150m, result[0].Value);
        // Second: (200 + 300) / 2 = 250
        Assert.AreEqual(250m, result[1].Value);
        // Third: (300 + 400) / 2 = 350
        Assert.AreEqual(350m, result[2].Value);
    }

    [TestMethod]
    public void Calculate_DifferentSelector_UsesCorrectProperty()
    {
        var data = new List<StockDataPoint>
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 10, 12, 9, 100, 1000),
            new StockDataPoint(new DateTime(2024, 1, 2), 20, 22, 19, 200, 2000),
            new StockDataPoint(new DateTime(2024, 1, 3), 30, 32, 29, 300, 3000),
            new StockDataPoint(new DateTime(2024, 1, 4), 40, 42, 39, 400, 4000),
            new StockDataPoint(new DateTime(2024, 1, 5), 50, 52, 49, 500, 5000)
        };
        var smaVolume = new SimpleMovingAverage(3, data => data.Volume);

        var result = smaVolume.Calculate(data);

        Assert.AreEqual(3, result.Count);
        // First: (1000 + 2000 + 3000) / 3 = 2000
        Assert.AreEqual(2000m, result[0].Value);
        // Second: (2000 + 3000 + 4000) / 3 = 3000
        Assert.AreEqual(3000m, result[1].Value);
        // Third: (3000 + 4000 + 5000) / 3 = 4000
        Assert.AreEqual(4000m, result[2].Value);
    }

    [TestMethod]
    public void Calculate_OpenPriceSelector_CalculatesCorrectly()
    {
        var data = new List<StockDataPoint>
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 10, 12, 9, 100, 1000),
            new StockDataPoint(new DateTime(2024, 1, 2), 20, 22, 19, 200, 2000),
            new StockDataPoint(new DateTime(2024, 1, 3), 30, 32, 29, 300, 3000),
            new StockDataPoint(new DateTime(2024, 1, 4), 40, 42, 39, 400, 4000),
            new StockDataPoint(new DateTime(2024, 1, 5), 50, 52, 49, 500, 5000)
        };
        var smaOpen = new SimpleMovingAverage(3, data => data.Open);

        var result = smaOpen.Calculate(data);

        Assert.AreEqual(3, result.Count);
        // First: (10 + 20 + 30) / 3 = 20
        Assert.AreEqual(20m, result[0].Value);
        // Second: (20 + 30 + 40) / 3 = 30
        Assert.AreEqual(30m, result[1].Value);
        // Third: (30 + 40 + 50) / 3 = 40
        Assert.AreEqual(40m, result[2].Value);
    }

    [TestMethod]
    public void Calculate_HighPriceSelector_CalculatesCorrectly()
    {
        var data = new List<StockDataPoint>
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 10, 100, 9, 50, 1000),
            new StockDataPoint(new DateTime(2024, 1, 2), 20, 200, 19, 50, 2000),
            new StockDataPoint(new DateTime(2024, 1, 3), 30, 300, 29, 50, 3000),
            new StockDataPoint(new DateTime(2024, 1, 4), 40, 400, 39, 50, 4000),
            new StockDataPoint(new DateTime(2024, 1, 5), 50, 500, 49, 50, 5000)
        };
        var smaHigh = new SimpleMovingAverage(3, data => data.High);

        var result = smaHigh.Calculate(data);

        Assert.AreEqual(3, result.Count);
        // First: (100 + 200 + 300) / 3 = 200
        Assert.AreEqual(200m, result[0].Value);
        // Second: (200 + 300 + 400) / 3 = 300
        Assert.AreEqual(300m, result[1].Value);
        // Third: (300 + 400 + 500) / 3 = 400
        Assert.AreEqual(400m, result[2].Value);
    }

    [TestMethod]
    public void Calculate_LowPriceSelector_CalculatesCorrectly()
    {
        var data = new List<StockDataPoint>
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 50, 100, 10, 50, 1000),
            new StockDataPoint(new DateTime(2024, 1, 2), 50, 200, 20, 50, 2000),
            new StockDataPoint(new DateTime(2024, 1, 3), 50, 300, 30, 50, 3000),
            new StockDataPoint(new DateTime(2024, 1, 4), 50, 400, 40, 50, 4000),
            new StockDataPoint(new DateTime(2024, 1, 5), 50, 500, 50, 50, 5000)
        };
        var smaLow = new SimpleMovingAverage(3, data => data.Low);

        var result = smaLow.Calculate(data);

        Assert.AreEqual(3, result.Count);
        // First: (10 + 20 + 30) / 3 = 20
        Assert.AreEqual(20m, result[0].Value);
        // Second: (20 + 30 + 40) / 3 = 30
        Assert.AreEqual(30m, result[1].Value);
        // Third: (30 + 40 + 50) / 3 = 40
        Assert.AreEqual(40m, result[2].Value);
    }

    [TestMethod]
    public void Calculate_DecimalValues_MaintainsPrecision()
    {
        var data = new List<StockDataPoint>
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 10.33m, 12, 9, 10.33m, 1000),
            new StockDataPoint(new DateTime(2024, 1, 2), 20.66m, 22, 19, 20.66m, 1000),
            new StockDataPoint(new DateTime(2024, 1, 3), 30.99m, 32, 29, 30.99m, 1000),
            new StockDataPoint(new DateTime(2024, 1, 4), 40.12m, 42, 39, 40.12m, 1000),
            new StockDataPoint(new DateTime(2024, 1, 5), 50.45m, 52, 49, 50.45m, 1000)
        };
        var sma = new SimpleMovingAverage(3, data => data.Close);

        var result = sma.Calculate(data);

        Assert.AreEqual(3, result.Count);
        // First: (10.33 + 20.66 + 30.99) / 3 = 20.66
        Assert.AreEqual(20.66m, result[0].Value);
        // Second: (20.66 + 30.99 + 40.12) / 3 = 30.59
        Assert.AreEqual(30.59m, result[1].Value);
        // Third: (30.99 + 40.12 + 50.45) / 3 = 40.52
        Assert.AreEqual(40.52m, result[2].Value);
    }

    [TestMethod]
    public void Calculate_PeriodEqualsDataCount_ReturnsSingleResult()
    {
        var data = CreateTestData(5);
        var sma = new SimpleMovingAverage(5, data => data.Close);

        var result = sma.Calculate(data);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        // Average of 100, 101, 102, 103, 104 = 102
        Assert.AreEqual(102m, result[0].Value);
    }

    [TestMethod]
    public void Calculate_LargeDataset_ProcessesCorrectly()
    {
        var data = CreateTestData(1000);
        var sma = new SimpleMovingAverage(20, data => data.Close);

        var result = sma.Calculate(data);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Count > 0);
    }

    [TestMethod]
    public void Calculate_ConsecutiveCalls_ReturnsSameResults()
    {
        var data = CreateTestData(10);
        var sma = new SimpleMovingAverage(3, data => data.Close);

        var result1 = sma.Calculate(data);
        var result2 = sma.Calculate(data);

        Assert.AreEqual(result1.Count, result2.Count);
        for (int i = 0; i < result1.Count; i++)
        {
            Assert.AreEqual(result1[i].Value, result2[i].Value);
            Assert.AreEqual(result1[i].Timestamp, result2[i].Timestamp);
        }
    }

    #endregion

    #region Calculate Method - Edge Cases

    [TestMethod]
    public void Calculate_MinimumDataForPeriod_ReturnsSingleResult()
    {
        var data = CreateTestData(3);
        var sma = new SimpleMovingAverage(3, data => data.Close);

        var result = sma.Calculate(data);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        // Average of 100, 101, 102 = 101
        Assert.AreEqual(101m, result[0].Value);
    }

    [TestMethod]
    public void Calculate_Period1_ReturnsMultipleResults()
    {
        var data = CreateTestData(5);
        var sma = new SimpleMovingAverage(1, data => data.Close);

        var result = sma.Calculate(data);

        // With period 1 and 5 data points: loop runs i=0 to i=4 (5 results)
        Assert.IsNotNull(result);
        Assert.AreEqual(5, result.Count);
        // Each result should equal the corresponding data point
        for (int i = 0; i < 5; i++)
        {
            Assert.AreEqual(100m + i, result[i].Value);
        }
    }

    [TestMethod]
    public void Calculate_AllZeroValues_CalculatesCorrectly()
    {
        var data = new List<StockDataPoint>
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 0, 0, 0, 0, 0),
            new StockDataPoint(new DateTime(2024, 1, 2), 0, 0, 0, 0, 0),
            new StockDataPoint(new DateTime(2024, 1, 3), 0, 0, 0, 0, 0),
            new StockDataPoint(new DateTime(2024, 1, 4), 0, 0, 0, 0, 0),
            new StockDataPoint(new DateTime(2024, 1, 5), 0, 0, 0, 0, 0)
        };
        var sma = new SimpleMovingAverage(3, data => data.Close);

        var result = sma.Calculate(data);

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(0m, result[0].Value);
        Assert.AreEqual(0m, result[1].Value);
        Assert.AreEqual(0m, result[2].Value);
    }

    [TestMethod]
    public void Calculate_AllSameValues_ReturnsConstantAverage()
    {
        var data = new List<StockDataPoint>
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 50, 50, 50, 50, 1000),
            new StockDataPoint(new DateTime(2024, 1, 2), 50, 50, 50, 50, 1000),
            new StockDataPoint(new DateTime(2024, 1, 3), 50, 50, 50, 50, 1000),
            new StockDataPoint(new DateTime(2024, 1, 4), 50, 50, 50, 50, 1000),
            new StockDataPoint(new DateTime(2024, 1, 5), 50, 50, 50, 50, 1000)
        };
        var sma = new SimpleMovingAverage(3, data => data.Close);

        var result = sma.Calculate(data);

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(50m, result[0].Value);
        Assert.AreEqual(50m, result[1].Value);
        Assert.AreEqual(50m, result[2].Value);
    }

    [TestMethod]
    public void Calculate_NegativeValues_CalculatesCorrectly()
    {
        var data = new List<StockDataPoint>
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 0, 0, -10, -10, 1000),
            new StockDataPoint(new DateTime(2024, 1, 2), 0, 0, -20, -20, 1000),
            new StockDataPoint(new DateTime(2024, 1, 3), 0, 0, -30, -30, 1000),
            new StockDataPoint(new DateTime(2024, 1, 4), 0, 0, -40, -40, 1000),
            new StockDataPoint(new DateTime(2024, 1, 5), 0, 0, -50, -50, 1000)
        };
        var sma = new SimpleMovingAverage(3, data => data.Close);

        var result = sma.Calculate(data);

        Assert.AreEqual(3, result.Count);
        // First: (-10 + -20 + -30) / 3 = -20
        Assert.AreEqual(-20m, result[0].Value);
        // Second: (-20 + -30 + -40) / 3 = -30
        Assert.AreEqual(-30m, result[1].Value);
        // Third: (-30 + -40 + -50) / 3 = -40
        Assert.AreEqual(-40m, result[2].Value);
    }

    [TestMethod]
    public void Calculate_VeryLargeValues_CalculatesCorrectly()
    {
        var data = new List<StockDataPoint>
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 1000000m, 1000000m, 1000000m, 1000000m, 1000),
            new StockDataPoint(new DateTime(2024, 1, 2), 2000000m, 2000000m, 2000000m, 2000000m, 1000),
            new StockDataPoint(new DateTime(2024, 1, 3), 3000000m, 3000000m, 3000000m, 3000000m, 1000),
            new StockDataPoint(new DateTime(2024, 1, 4), 4000000m, 4000000m, 4000000m, 4000000m, 1000),
            new StockDataPoint(new DateTime(2024, 1, 5), 5000000m, 5000000m, 5000000m, 5000000m, 1000)
        };
        var sma = new SimpleMovingAverage(3, data => data.Close);

        var result = sma.Calculate(data);

        Assert.AreEqual(3, result.Count);
        // First: (1M + 2M + 3M) / 3 = 2M
        Assert.AreEqual(2000000m, result[0].Value);
        // Second: (2M + 3M + 4M) / 3 = 3M
        Assert.AreEqual(3000000m, result[1].Value);
        // Third: (3M + 4M + 5M) / 3 = 4M
        Assert.AreEqual(4000000m, result[2].Value);
    }

    #endregion

    #region Validation Tests

    [TestMethod]
    public void Calculate_NullData_ThrowsArgumentNullException()
    {
        var sma = new SimpleMovingAverage(3, data => data.Close);

        Action act = () => sma.Calculate(null!);

        Assert.ThrowsExactly<ArgumentNullException>(act);
    }

    [TestMethod]
    public void Calculate_EmptyData_ThrowsArgumentException()
    {
        var sma = new SimpleMovingAverage(3, data => data.Close);
        var data = new List<StockDataPoint>();

        Action act = () => sma.Calculate(data);
        
        Assert.ThrowsExactly<ArgumentException>(act);
    }

    [TestMethod]
    public void Calculate_PeriodZero_ThrowsArgumentException()
    {
        var data = CreateTestData(5);
        var sma = new SimpleMovingAverage(0, data => data.Close);

        Action act = () => sma.Calculate(data);

        Assert.ThrowsExactly<ArgumentException>(act);
    }

    [TestMethod]
    public void Calculate_NegativePeriod_ThrowsArgumentException()
    {
        var data = CreateTestData(5);
        var sma = new SimpleMovingAverage(-5, data => data.Close);

        Action act = () => sma.Calculate(data);

        Assert.ThrowsExactly<ArgumentException>(act);
    }

    [TestMethod]
    public void Calculate_PeriodExceedsDataCount_ThrowsArgumentException()
    {
        var data = CreateTestData(5);
        var sma = new SimpleMovingAverage(10, data => data.Close);

        Action act = () => sma.Calculate(data);

        Assert.ThrowsExactly<ArgumentException>(act);
    }

    [TestMethod]
    public void Calculate_UnsortedData_ThrowsArgumentException()
    {
        var data = new List<StockDataPoint>
        {
            new StockDataPoint(new DateTime(2024, 1, 3), 30, 32, 29, 30, 1000),
            new StockDataPoint(new DateTime(2024, 1, 1), 10, 12, 9, 10, 1000),
            new StockDataPoint(new DateTime(2024, 1, 2), 20, 22, 19, 20, 1000),
            new StockDataPoint(new DateTime(2024, 1, 4), 40, 42, 39, 40, 1000)
        };
        var sma = new SimpleMovingAverage(3, data => data.Close);

        Action act = () => sma.Calculate(data);

        Assert.ThrowsExactly<ArgumentException>(act);
    }

    [TestMethod]
    public void Calculate_DataWithDuplicateTimestamps_ProcessesSuccessfully()
    {
        // Note: Implementation only validates chronological order (>=), allowing duplicates
        var data = new List<StockDataPoint>
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 10, 12, 9, 10, 1000),
            new StockDataPoint(new DateTime(2024, 1, 1), 20, 22, 19, 20, 1000),
            new StockDataPoint(new DateTime(2024, 1, 2), 30, 32, 29, 30, 1000),
            new StockDataPoint(new DateTime(2024, 1, 3), 40, 42, 39, 40, 1000),
            new StockDataPoint(new DateTime(2024, 1, 4), 50, 52, 49, 50, 1000)
        };
        var sma = new SimpleMovingAverage(3, data => data.Close);

        var result = sma.Calculate(data);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Count >= 0);
    }

    #endregion

    #region Timestamp Tests

    [TestMethod]
    public void Calculate_VerifyTimestampAlignment()
    {
        var data = new List<StockDataPoint>
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 10, 12, 9, 10, 1000),
            new StockDataPoint(new DateTime(2024, 1, 2), 20, 22, 19, 20, 1000),
            new StockDataPoint(new DateTime(2024, 1, 3), 30, 32, 29, 30, 1000),
            new StockDataPoint(new DateTime(2024, 1, 4), 40, 42, 39, 40, 1000),
            new StockDataPoint(new DateTime(2024, 1, 5), 50, 52, 49, 50, 1000),
            new StockDataPoint(new DateTime(2024, 1, 6), 60, 62, 59, 60, 1000)
        };
        var sma = new SimpleMovingAverage(3, data => data.Close);

        var result = sma.Calculate(data);

        Assert.AreEqual(4, result.Count);
        // First result: i=0, timestamp = data[0 + 3 - 1] = data[2]
        Assert.AreEqual(new DateTime(2024, 1, 3), result[0].Timestamp);
        // Second result: i=1, timestamp = data[1 + 3 - 1] = data[3]
        Assert.AreEqual(new DateTime(2024, 1, 4), result[1].Timestamp);
        // Third result: i=2, timestamp = data[2 + 3 - 1] = data[4]
        Assert.AreEqual(new DateTime(2024, 1, 5), result[2].Timestamp);
        // Fourth result: i=3, timestamp = data[3 + 3 - 1] = data[5]
        Assert.AreEqual(new DateTime(2024, 1, 6), result[3].Timestamp);
    }

    [TestMethod]
    public void Calculate_ChronologicalOrder_MaintainsOrder()
    {
        var data = CreateTestData(10);
        var sma = new SimpleMovingAverage(3, data => data.Close);

        var result = sma.Calculate(data);
        
        for (int i = 1; i < result.Count; i++)
        {
            Assert.IsTrue(result[i].Timestamp > result[i - 1].Timestamp);
        }
    }

    #endregion
}
