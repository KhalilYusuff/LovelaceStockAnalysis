using LovelaceGroup6.src.StockAnalysis.Indicators;
using LovelaceGroup6.StockAnalysis.Models;
using LovelaceGroup6.Tests.Utilities;

namespace LovelaceGroup6.Tests.Indicators;

[TestClass]
public class RelativeStrengthIndexTest
{
    private static List<StockDataPoint> CreateTestData(int count, decimal basePrice = 100m)
    {
        var data = new List<StockDataPoint>();
        var startDate = new DateTime(2024, 1, 1);

        for (int i = 0; i < count; i++)
        {
            var price = basePrice + i;
            data.Add(StockDataPointFactory.Create(
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

    private static List<IReadOnlyList<StockDataPoint>> Wrap(List<StockDataPoint> data)
        => new List<IReadOnlyList<StockDataPoint>> { data };

    #region Constructor Tests

    [TestMethod]
    public void Constructor_ValidParameters_CreatesInstance()
    {
        var rsi = new RelativeStrengthIndex(14, data => data.Close);

        Assert.IsNotNull(rsi);
    }

    [TestMethod]
    public void Constructor_NullSelector_ThrowsArgumentNullException()
    {
        Action act = () => new RelativeStrengthIndex(14, null!);

        Assert.ThrowsExactly<ArgumentNullException>(act);
    }

    [TestMethod]
    public void Constructor_ZeroPeriod_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => new RelativeStrengthIndex(0, data => data.Close);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
    }

    [TestMethod]
    public void Constructor_NegativePeriod_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => new RelativeStrengthIndex(-5, data => data.Close);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
    }

    #endregion

    #region Calculate Method - Valid Cases

    [TestMethod]
    public void Calculate_ValidData_ReturnsCorrectCount()
    {
        var data = CreateTestData(20, 100m);
        var rsi = new RelativeStrengthIndex(14, data => data.Close);

        var result = rsi.Calculate(Wrap(data));

        Assert.IsNotNull(result);
        Assert.AreEqual(6, result.Count);
    }

    [TestMethod]
    public void Calculate_Period1_RequiresMinimumDataPoints()
    {
        var data = CreateTestData(3, 100m);
        var rsi = new RelativeStrengthIndex(1, data => data.Close);

        var result = rsi.Calculate(Wrap(data));

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void Calculate_AllGains_ReturnsRsiOf100()
    {
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 1), close: 100m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 2), close: 101m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 3), close: 102m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 4), close: 103m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 5), close: 104m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 6), close: 105m)
        };
        var rsi = new RelativeStrengthIndex(2, data => data.Close);

        var result = rsi.Calculate(Wrap(data));

        Assert.IsTrue(result.All(r => r.Value == 100m));
    }

    [TestMethod]
    public void Calculate_AllLosses_ReturnsRsiOf0()
    {
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 1), close: 105m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 2), close: 104m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 3), close: 103m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 4), close: 102m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 5), close: 101m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 6), close: 100m)
        };
        var rsi = new RelativeStrengthIndex(2, data => data.Close);

        var result = rsi.Calculate(Wrap(data));

        Assert.IsTrue(result.All(r => r.Value == 0m));
    }

    [TestMethod]
    public void Calculate_ConstantPrice_ReturnsRsiOf100()
    {
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 1), close: 100m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 2), close: 100m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 3), close: 100m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 4), close: 100m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 5), close: 100m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 6), close: 100m)
        };
        var rsi = new RelativeStrengthIndex(2, data => data.Close);

        var result = rsi.Calculate(Wrap(data));

        Assert.IsTrue(result.All(r => r.Value == 100m));
    }

    [TestMethod]
    public void Calculate_DifferentSelector_UsesCorrectProperty()
    {
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 1), open: 100m, high: 105m, low: 95m, close: 100m, volume: 1000),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 2), open: 101m, high: 106m, low: 96m, close: 105m, volume: 1100),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 3), open: 102m, high: 107m, low: 97m, close: 103m, volume: 1200),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 4), open: 103m, high: 108m, low: 98m, close: 108m, volume: 1300),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 5), open: 104m, high: 109m, low: 99m, close: 107m, volume: 1400)
        };
        var rsiHigh = new RelativeStrengthIndex(2, data => data.High);

        var result = rsiHigh.Calculate(Wrap(data));

        Assert.IsNotNull(result);
        Assert.IsTrue(result.All(r => r.Value >= 0 && r.Value <= 100));
    }

    [TestMethod]
    public void Calculate_TimestampAlignment()
    {
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 1), close: 100m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 2), close: 102m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 3), close: 101m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 4), close: 103m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 5), close: 105m)
        };
        var rsi = new RelativeStrengthIndex(2, data => data.Close);

        var result = rsi.Calculate(Wrap(data));

        Assert.AreEqual(new DateTime(2024, 1, 3), result[0].Timestamp);
        Assert.AreEqual(new DateTime(2024, 1, 4), result[1].Timestamp);
        Assert.AreEqual(new DateTime(2024, 1, 5), result[2].Timestamp);
    }

    [TestMethod]
    public void Calculate_ConsecutiveCalls_ReturnsSameResults()
    {
        var data = CreateTestData(20);
        var rsi = new RelativeStrengthIndex(14, data => data.Close);

        var result1 = rsi.Calculate(Wrap(data));
        var result2 = rsi.Calculate(Wrap(data));

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
        var data = CreateTestData(15, 100m);
        var rsi = new RelativeStrengthIndex(14, data => data.Close);

        var result = rsi.Calculate(Wrap(data));

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void Calculate_LargeDataset_ProcessesCorrectly()
    {
        var data = CreateTestData(1000, 100m);
        var rsi = new RelativeStrengthIndex(14, data => data.Close);

        var result = rsi.Calculate(Wrap(data));

        Assert.IsNotNull(result);
        Assert.AreEqual(986, result.Count);
        Assert.IsTrue(result.All(r => r.Value >= 0 && r.Value <= 100));
    }

    [TestMethod]
    public void Calculate_VerySmallPeriod_CalculatesCorrectly()
    {
        var data = CreateTestData(5, 100m);
        var rsi = new RelativeStrengthIndex(1, data => data.Close);

        var result = rsi.Calculate(Wrap(data));

        Assert.IsNotNull(result);
        Assert.AreEqual(4, result.Count);
        Assert.IsTrue(result.All(r => r.Value >= 0 && r.Value <= 100));
    }

    [TestMethod]
    public void Calculate_MixedGainsAndLosses_ReturnsValidRsi()
    {
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 1), close: 100m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 2), close: 102m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 3), close: 99m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 4), close: 101m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 5), close: 100m)
        };
        var rsi = new RelativeStrengthIndex(2, data => data.Close);

        var result = rsi.Calculate(Wrap(data));

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count);
        Assert.IsTrue(result.All(r => r.Value >= 0 && r.Value <= 100));
    }

    [TestMethod]
    public void Calculate_RsiValueRange_AlwaysBetween0And100()
    {
        var data = CreateTestData(50, 100m);
        var rsi = new RelativeStrengthIndex(14, data => data.Close);

        var result = rsi.Calculate(Wrap(data));

        Assert.IsTrue(result.All(r => r.Value >= 0 && r.Value <= 100),
            "RSI values should always be between 0 and 100");
    }

    [TestMethod]
    public void Calculate_NegativePrice_CalculatesCorrectly()
    {
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 1), close: -100m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 2), close: -98m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 3), close: -99m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 4), close: -97m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 5), close: -96m)
        };
        var rsi = new RelativeStrengthIndex(2, data => data.Close);

        var result = rsi.Calculate(Wrap(data));

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count);
        Assert.IsTrue(result.All(r => r.Value >= 0 && r.Value <= 100));
    }

    [TestMethod]
    public void Calculate_DecimalPrecision_MaintainsPrecision()
    {
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 1), close: 100.123m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 2), close: 100.456m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 3), close: 100.789m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 4), close: 100.321m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 5), close: 100.654m)
        };
        var rsi = new RelativeStrengthIndex(2, data => data.Close);

        var result = rsi.Calculate(Wrap(data));

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count);
        Assert.IsTrue(result.All(r => r.Value >= 0 && r.Value <= 100));
    }

    [TestMethod]
    public void Calculate_WildersSmoothing_ProducesConsistentResults()
    {
        var data = CreateTestData(30, 100m);
        var rsi = new RelativeStrengthIndex(14, data => data.Close);

        var result = rsi.Calculate(Wrap(data));

        Assert.IsNotNull(result);
        Assert.AreEqual(16, result.Count);
        Assert.IsTrue(result.All(r => r.Value >= 0 && r.Value <= 100));

        var firstHalf = result.Take(result.Count / 2).Average(r => r.Value);
        var secondHalf = result.Skip(result.Count / 2).Average(r => r.Value);
        Assert.IsTrue(secondHalf >= firstHalf,
            "In an uptrend, RSI should generally increase over time");
    }

    #endregion

    #region Validation Tests

    [TestMethod]
    public void Calculate_NullData_ThrowsArgumentNullException()
    {
        var rsi = new RelativeStrengthIndex(14, data => data.Close);

        Action act = () => rsi.Calculate((IReadOnlyList<IReadOnlyList<StockDataPoint>>)null!);

        Assert.ThrowsExactly<ArgumentNullException>(act);
    }

    [TestMethod]
    public void Calculate_EmptyData_ThrowsArgumentException()
    {
        var rsi = new RelativeStrengthIndex(14, data => data.Close);

        Action act = () => rsi.Calculate(new List<IReadOnlyList<StockDataPoint>>());

        Assert.ThrowsExactly<ArgumentException>(act);
    }

    [TestMethod]
    public void Calculate_InsufficientDataForPeriod_ThrowsArgumentException()
    {
        var data = CreateTestData(10, 100m);
        var rsi = new RelativeStrengthIndex(14, data => data.Close);

        Action act = () => rsi.Calculate(Wrap(data));

        Assert.ThrowsExactly<ArgumentException>(act);
    }

    [TestMethod]
    public void Calculate_PeriodExceedsDataCount_ThrowsArgumentException()
    {
        var data = CreateTestData(5, 100m);
        var rsi = new RelativeStrengthIndex(20, data => data.Close);

        Action act = () => rsi.Calculate(Wrap(data));

        Assert.ThrowsExactly<ArgumentException>(act);
    }

    [TestMethod]
    public void Calculate_UnsortedData_ThrowsArgumentException()
    {
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 3), close: 103m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 1), close: 101m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 2), close: 102m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 4), close: 104m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 5), close: 105m)
        };
        var rsi = new RelativeStrengthIndex(2, data => data.Close);

        Action act = () => rsi.Calculate(Wrap(data));

        Assert.ThrowsExactly<ArgumentException>(act);
    }

    #endregion
}