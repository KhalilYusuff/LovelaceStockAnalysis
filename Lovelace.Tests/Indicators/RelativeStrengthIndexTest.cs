using LovelaceGroup6.src.StockAnalysis.Indicators;
using LovelaceGroup6.StockAnalysis.Models;
using LovelaceGroup6.Tests.Utilities;

namespace LovelaceGroup6.Tests.Indicators;

[TestClass]

// Initial version of this test class was generated using GitHub Copilot (Visual Studio).
// The class has since been simplified and reduced from 25+ tests to 5 tests.
// See attached log: copilot-log-rsi-tests.docx

public class RelativeStrengthIndexTest
{
    private static List<IReadOnlyList<StockDataPoint>> Wrap(List<StockDataPoint> data)
        => new List<IReadOnlyList<StockDataPoint>> { data };

    [TestMethod]
    public void Constructor_NullSelector_ThrowsArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new RelativeStrengthIndex(14, null!);
        // Assert
        Assert.ThrowsExactly<ArgumentNullException>(act);
    }

    [TestMethod]
    public void Constructor_ZeroPeriod_ThrowsArgumentException()
    {
        // Arrange & Act
        Action act = () => new RelativeStrengthIndex(0, x => x.Close);
        // Assert
        Assert.ThrowsExactly<ArgumentException>(act);
    }

    [TestMethod]
    public void Calculate_KnownValues_ReturnsCorrectRsi()
    {
        // Arrange
        const int period = 2;
        const double expectedRsi = 66.6666666666667;
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 1), close: 100m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 2), close: 102m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 3), close: 101m)
        };
        var rsi = new RelativeStrengthIndex(period, x => x.Close);
        // Act
        var result = rsi.Calculate(Wrap(data));
        // Assert
        Assert.AreEqual(expectedRsi, (double)result[0].Value, delta: 0.0000001);
    }

    [TestMethod]
    public void Calculate_NullData_ThrowsArgumentNullException()
    {
        // Arrange
        var rsi = new RelativeStrengthIndex(14, x => x.Close);
        // Act
        Action act = () => rsi.Calculate((IReadOnlyList<IReadOnlyList<StockDataPoint>>)null!);
        // Assert
        Assert.ThrowsExactly<ArgumentNullException>(act);
    }

    [TestMethod]
    public void Calculate_InsufficientDataForPeriod_ThrowsArgumentException()
    {
        // Arrange
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 1), close: 100m),
            StockDataPointFactory.Create(timestamp: new DateTime(2024, 1, 2), close: 101m)
        };
        var rsi = new RelativeStrengthIndex(14, x => x.Close);
        // Act
        Action act = () => rsi.Calculate(Wrap(data));
        // Assert
        Assert.ThrowsExactly<ArgumentException>(act);
    }
}