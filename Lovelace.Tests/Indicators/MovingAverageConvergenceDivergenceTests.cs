using Lovelace.StockAnalysis.Indicators;
using Lovelace.StockAnalysis.Tests.Utilities;

namespace Lovelace.StockAnalysis.Tests.Indicators;

[TestClass]
public class MovingAverageConvergenceDivergenceTests
{
    private static List<StockDataPoint> CreateTestData(int count, decimal basePrice = 100m)
    {
        var data = new List<StockDataPoint>();
        var startDate = new DateTime(2024, 1, 1);

        for (int i = 0; i < count; i++)
        {
            var price = basePrice + i;
            data.Add(StockDataPointFactory.Create(
                open: price,
                high: price + 2,
                low: price - 1,
                close: price,
                volume: 1000m,
                timestamp: startDate.AddDays(i)
            ));
        }

        return data;
    }

    //Constructor Tests

    [TestMethod]
    public void Constructor_ValidParameters_CreatesInstance()
    {
        var macd = new MovingAverageConvergenceDivergence(12, 26, 9, p => p.Close);
        Assert.IsNotNull(macd);
    }

    [TestMethod]
    public void Constructor_NullSelector_ThrowsArgumentNullException()
    {
        Action act = () => new MovingAverageConvergenceDivergence(12, 26, 9, null!);
        Assert.ThrowsExactly<ArgumentNullException>(act);
    }

    [TestMethod]
    public void Constructor_ZeroPeriod_ThrowsArgumentException()
    {
        Action act = () => new MovingAverageConvergenceDivergence(0, 26, 9, p => p.Close);
        Assert.ThrowsExactly<ArgumentException>(act);
    }

    [TestMethod]
    public void Constructor_FastPeriodGreaterThanSlow_ThrowsArgumentException()
    {
        Action act = () => new MovingAverageConvergenceDivergence(30, 26, 9, p => p.Close);
        Assert.ThrowsExactly<ArgumentException>(act);
    }


    // Calculate Method - Valid Cases

    [TestMethod]
    public void Calculate_StandardPeriods_ReturnsThreeSeries()
    {
        var macd = new MovingAverageConvergenceDivergence(12, 26, 9, p => p.Close);
        var data = CreateTestData(50);

        var result = macd.Calculate(data);

        Assert.IsTrue(result.ContainsKey(MovingAverageConvergenceDivergence.MacdLineName));
        Assert.IsTrue(result.ContainsKey(MovingAverageConvergenceDivergence.SignalLineName));
        Assert.IsTrue(result.ContainsKey(MovingAverageConvergenceDivergence.HistogramName));
    }

    [TestMethod]
    public void Calculate_StandardPeriods_ReturnsExpectedCount()
    {
        var macd = new MovingAverageConvergenceDivergence(12, 26, 9, p => p.Close);
        var data = CreateTestData(50);

        var result = macd.Calculate(data);

        int expectedCount = data.Count - 26 - 9 + 2;
        var count = result[MovingAverageConvergenceDivergence.MacdLineName].Count;
        Assert.AreEqual(expectedCount, count);

    }

    [TestMethod]
    public void Calculate_HistogramEqualsMacdMinusSignal()
    {
        var macd = new MovingAverageConvergenceDivergence(12, 26, 9, p => p.Close);
        var data = CreateTestData(50);

        var result = macd.Calculate(data);
        var macdLine = result[MovingAverageConvergenceDivergence.MacdLineName];
        var signalLine = result[MovingAverageConvergenceDivergence.SignalLineName];
        var histogram = result[MovingAverageConvergenceDivergence.HistogramName];

        for (int i = 0; i < histogram.Count; i++)
        {
            Assert.AreEqual(macdLine[i].Value - signalLine[i].Value, histogram[i].Value);
        }
    }

    [TestMethod]
    public void Calculate_MinimumDataPoints_ReturnsOneResult()
    {
        var macd = new MovingAverageConvergenceDivergence(12, 26, 9, p => p.Close);
        var data = CreateTestData(34);

        var result = macd.Calculate(data);

        var count = result[MovingAverageConvergenceDivergence.MacdLineName].Count;
        Assert.AreEqual(1, count);
    }


    // Timestamp Tests

    [TestMethod]
    public void Calculate_AllSeriesShareSameTimestamps()
    {
        var macd = new MovingAverageConvergenceDivergence(12, 26, 9, p => p.Close);
        var data = CreateTestData(50);

        var result = macd.Calculate(data);
        var macdLine = result[MovingAverageConvergenceDivergence.MacdLineName];
        var signalLine = result[MovingAverageConvergenceDivergence.SignalLineName];
        var histogram = result[MovingAverageConvergenceDivergence.HistogramName];

        for (int i = 0; i < macdLine.Count; i++)
        {
            Assert.AreEqual(macdLine[i].Timestamp, signalLine[i].Timestamp);
            Assert.AreEqual(macdLine[i].Timestamp, histogram[i].Timestamp);
        }
    }

}
