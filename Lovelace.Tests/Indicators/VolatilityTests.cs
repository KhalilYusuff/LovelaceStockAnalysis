using Lovelace.StockAnalysis.Indicators;
using Lovelace.StockAnalysis.Models;
using Lovelace.StockAnalysis.Tests.Utilities;
using Lovelace.StockAnalysis.Core.Exceptions;

namespace Lovelace.StockAnalysis.Tests.Indicators;

[TestClass]
public class VolatilityTests
{

    [TestMethod]
    public void Calculate_KnownValues_ReturnsCorrectStandardDeviation()
    {
        const int period = 3;
        const decimal expectedStandardDeviation = 2m;
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(open: 1m, close: 0m),
            StockDataPointFactory.Create(open: 1m, close: 2m),
            StockDataPointFactory.Create(open: 1m, close: 4m)
        };
        var volatility = new Volatility(period, x => x.Close);

        var results = volatility.Calculate(data);

        Assert.AreEqual(expectedStandardDeviation, results[0].Value);
    }

    [TestMethod]
    public void Calculate_SelectorIsApplied_UsesCorrectPrice()
    {
        const int period = 3;
        const decimal expectedStandardDeviation = 2m;
        var data = new List<StockDataPoint>
        {
            StockDataPointFactory.Create(open: 0m, close: 10m),
            StockDataPointFactory.Create(open: 2m, close: 10m),
            StockDataPointFactory.Create(open: 4m, close: 10m)
        };
        var volatility = new Volatility(period, x => x.Open);

        var results = volatility.Calculate(data);

        Assert.AreEqual(expectedStandardDeviation, results[0].Value);
    }
}
