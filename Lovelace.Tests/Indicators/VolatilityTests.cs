using Lovelace.StockAnalysis.Indicators;
using Lovelace.StockAnalysis.Models;
using Lovelace.StockAnalysis.Tests.Utilities;

namespace Lovelace.StockAnalysis.Tests.Indicators;

[TestClass]
public class VolatilityTests
{
    [TestMethod]
    public void Constructor_PeriodLessThan2_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const int invalidPeriod = 1;

        // Act
        Action act = () => new Volatility(invalidPeriod, x => x.Close);

        // Assert
        Assert.ThrowsExactly<ArgumentException>(act);
    }

    [TestMethod]
    public void Constructor_NullSelector_ThrowsArgumentNullException()
    {
        // Arrange
        const int period = 3;

        // Act
        Action act = () => new Volatility(period, null!);

        // Assert
        Assert.ThrowsExactly<ArgumentNullException>(act);
    }


    [TestMethod]
    public void Calculate_KnownValues_ReturnsCorrectStandardDeviation()
    {
        // Arrange
        const int period = 3;
        const decimal expectedStandardDeviation = 2m;
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(open: 1m, close: 0m),
            StockDataPointFactory.Create(open: 1m, close: 2m),
            StockDataPointFactory.Create(open: 1m, close: 4m)
        };
        var volatility = new Volatility(period, x => x.Close);

        // Act
        var results = volatility.Calculate(data);

        // Assert
        Assert.AreEqual(expectedStandardDeviation, results[0].Value);
    }

    [TestMethod]
    public void Calculate_SelectorIsApplied_UsesCorrectPrice()
    {
        // Arrange
        const int period = 3;
        const decimal expectedStandardDeviation = 2m;
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(open: 0m, close: 10m),
            StockDataPointFactory.Create(open: 2m, close: 10m),
            StockDataPointFactory.Create(open: 4m, close: 10m)
        };
        var volatility = new Volatility(period, x => x.Open);

        // Act
        var results = volatility.Calculate(data);

        // Assert
        Assert.AreEqual(expectedStandardDeviation, results[0].Value);
    }
}
