using Lovelace.StockAnalysis;
using Lovelace.StockAnalysis.Indicators;

namespace Lovelace.Tests.Scenarios;

[TestClass]
public class IndicatorScenarioTests
{

    [TestMethod]
    public void Should_SelectTheBestStockAmongMultipleCandidates_BasedOnIndicatorValue()
    {

        var start = new DateTime(2024, 1, 1);
        var returnCalculation = new ReturnCalculation( p => p.Close);
        var totalReturnCalculations = new Dictionary<string, decimal>();

        
        var stock1 = new List<StockDataPoint>
        {
            new StockDataPoint(start, 100m, 104m, 99m, 103m, 1500m),
            new StockDataPoint(start.AddDays(1), 103m, 106m, 102m, 105m, 1600m),
            new StockDataPoint(start.AddDays(2), 105m, 107m, 104m, 106m, 1700m),
            new StockDataPoint(start.AddDays(3), 106m, 108m, 105m, 107m, 1650m),
            new StockDataPoint(start.AddDays(4), 107m, 110m, 106m, 109m, 1800m),
        };
        
        var stock2 = new List<StockDataPoint>
        {
            new StockDataPoint(start, 200m, 206m, 198m, 205m, 2200m),
            new StockDataPoint(start.AddDays(1), 205m, 207m, 201m, 202m, 2100m),
            new StockDataPoint(start.AddDays(2), 202m, 204m, 197m, 198m, 2300m),
            new StockDataPoint(start.AddDays(3), 198m, 200m, 194m, 196m, 2400m),
            new StockDataPoint(start.AddDays(4), 196m, 199m, 193m, 194m, 2500m),
        };

        var stock3 = new List<StockDataPoint>
        {
            new StockDataPoint(start ,50m, 51m, 49m, 50m, 5000m),
            new StockDataPoint(start.AddDays(1), 50m, 52m, 48m, 51m, 5200m),
            new StockDataPoint(start.AddDays(2), 51m, 53m, 50m, 52m, 5100m),
            new StockDataPoint(start.AddDays(3), 52m, 54m, 51m, 53m, 5300m),
            new StockDataPoint(start.AddDays(4), 53m, 55m, 52m, 54m, 5400m),
        };

        var stock4 = new List<StockDataPoint>
        {
            new StockDataPoint(start, 300m, 303m, 296m, 297m, 1800m),
            new StockDataPoint(start.AddDays(1), 297m, 300m, 294m, 299m, 1750m),
            new StockDataPoint(start.AddDays(2), 299m, 301m, 295m, 296m, 1900m),
            new StockDataPoint(start.AddDays(3), 296m, 298m, 292m, 293m, 2000m),
            new StockDataPoint(start.AddDays(4), 293m, 295m, 289m, 290m, 2100m),
        };

        var stock5 = new List<StockDataPoint>
        {
            new StockDataPoint(start, 80m, 85m, 79m, 84m, 3000m),
            new StockDataPoint(start.AddDays(1), 84m, 88m, 83m, 87m, 3200m),
            new StockDataPoint(start.AddDays(2), 87m, 90m, 86m, 89m, 3400m),
            new StockDataPoint(start.AddDays(3), 89m, 91m, 88m, 90m, 3600m),
            new StockDataPoint(start.AddDays(4), 90m, 94m, 89m, 93m, 3800m),
        };

        var stock6 = new List<StockDataPoint>
        {
            new StockDataPoint(start, 150m, 152m, 148m, 151m, 2600m),
            new StockDataPoint(start.AddDays(1), 151m, 153m, 149m, 150m, 2550m),
            new StockDataPoint(start.AddDays(2), 150m, 151m, 147m, 148m, 2650m),
            new StockDataPoint(start.AddDays(3), 148m, 149m, 145m, 146m, 2700m),
            new StockDataPoint(start.AddDays(4), 146m, 147m, 143m, 144m, 2750m),
        };
        var stock1Result = returnCalculation.Calculate(stock1);
        var stock2Result = returnCalculation.Calculate(stock2);
        var stock3Result = returnCalculation.Calculate(stock3);
        var stock4Result = returnCalculation.Calculate(stock4);
        var stock5Result = returnCalculation.Calculate(stock5);
        var stock6Result = returnCalculation.Calculate(stock6);

        var stock1TotalReturn = 1m;
        foreach (var i in stock1Result)
        {
            stock1TotalReturn *= (1m + i.Value);
        }
        stock1TotalReturn -= 1m;

        var stock2TotalReturn = 1m;
        foreach (var i in stock2Result)
        {
            stock2TotalReturn *= (1m + i.Value);
        }
        stock2TotalReturn -= 1m;

        var stock3TotalReturn = 1m;
        foreach (var i in stock3Result)
        {
            stock3TotalReturn *= (1m + i.Value);
        }
        stock3TotalReturn -= 1m;
        
        var stock4TotalReturn = 1m;
        foreach (var i in stock4Result)
        {
            stock4TotalReturn *= (1m + i.Value);
        }
        stock4TotalReturn -= 1m; 
        
        var stock5TotalReturn = 1m;
        foreach (var i in stock5Result)
        {
            stock5TotalReturn *= (1m + i.Value);
        }
        stock5TotalReturn -= 1m;
        
        var stock6TotalReturn = 1m;
        foreach (var i in stock6Result)
        {
            stock6TotalReturn *= (1m + i.Value);
        }
        stock6TotalReturn -= 1m;

        totalReturnCalculations.Add("stock1", stock1TotalReturn);
        totalReturnCalculations.Add("stock2", stock2TotalReturn);
        totalReturnCalculations.Add("stock3", stock3TotalReturn);
        totalReturnCalculations.Add("stock4", stock4TotalReturn);
        totalReturnCalculations.Add("stock5", stock5TotalReturn);
        totalReturnCalculations.Add("stock6", stock6TotalReturn);

        var bestStock = totalReturnCalculations.OrderByDescending(x => x.Value).First();
        
        Assert.AreEqual("stock5", bestStock.Key);
        Assert.AreEqual(start.AddDays(1), stock5Result[0].Timestamp);
    }

    [TestMethod]
    public void Should_PreserveTimestampForEachCalculatedIndicatorValue()
    {
        var sma = new SimpleMovingAverage(2, p => p.Close);
        var start  = new DateTime(2023, 1, 1);
        var stock = new List<StockDataPoint>
        {
            new StockDataPoint(start, 100m, 104m, 99m, 103m, 1500m),
            new StockDataPoint(start.AddDays(1), 103m, 106m, 102m, 105m, 1600m),
            new StockDataPoint(start.AddDays(2), 105m, 107m, 104m, 106m, 1700m),
            new StockDataPoint(start.AddDays(3), 106m, 108m, 105m, 107m, 1650m),
            new StockDataPoint(start.AddDays(4), 107m, 110m, 106m, 109m, 1800m),
        };

        var result = sma.Calculate(stock);
        
        Assert.AreEqual(new DateTime(2023, 1, 2), result[0].Timestamp);
        Assert.AreEqual(new DateTime(2023, 1, 3), result[1].Timestamp);
    }

    [TestMethod]
    public void Should_DetectWhenEMACrossesUnderSMA()
    {
        var sma = new SimpleMovingAverage(3, p => p.Close);
        var ema = new ExponentialMovingAverage(3, p => p.Close);
        var stockData = new List<StockDataPoint>
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 0m, 0m, 0m, 10m, 0m),
            new StockDataPoint(new DateTime(2024, 1, 2), 0m, 0m, 0m, 9m, 0m),
            new StockDataPoint(new DateTime(2024, 1, 3), 0m, 0m, 0m, 8m, 0m),
            new StockDataPoint(new DateTime(2024, 1, 4), 0m, 0m, 0m, 9m, 0m),
            new StockDataPoint(new DateTime(2024, 1, 5), 0m, 0m, 0m, 10m, 0m),
            new StockDataPoint(new DateTime(2024, 1, 6), 0m, 0m, 0m, 11m, 0m),
            new StockDataPoint(new DateTime(2024, 1, 7), 0m, 0m, 0m, 10m, 0m),
            new StockDataPoint(new DateTime(2024, 1, 8), 0m, 0m, 0m, 9m, 0m),
            new StockDataPoint(new DateTime(2024, 1, 9), 0m, 0m, 0m, 8m, 0m),
        };
        var smaResult = sma.Calculate(stockData);
        var emaResult = ema.Calculate(stockData);

        for (int i = 1; i < emaResult.Count - 1; i++)
        {
            var previousSMAValue = smaResult[i - 1].Value;
            var currentSMAVale = smaResult[i].Value;

            var previousEMAValue = emaResult[i - 1].Value;
            var currentEMAValue = emaResult[i].Value;
            
            if (previousEMAValue >= previousSMAValue
                     && currentEMAValue < currentSMAVale)
            {
                var emaCrossedUnderSmaAt = emaResult[i].Timestamp;
                Assert.AreEqual(new DateTime(2024, 1, 7), emaCrossedUnderSmaAt);
            }
        }

    }

    [TestMethod]
    public void Should_DetectWhenEMACrossesAboveSMA()
    {
        var sma = new SimpleMovingAverage(3, p => p.Close);
        var ema = new ExponentialMovingAverage(3, p => p.Close);
        var stockData = new List<StockDataPoint>
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 0m, 0m, 0m, 10m, 0m),
            new StockDataPoint(new DateTime(2024, 1, 2), 0m, 0m, 0m, 9m, 0m),
            new StockDataPoint(new DateTime(2024, 1, 3), 0m, 0m, 0m, 8m, 0m),
            new StockDataPoint(new DateTime(2024, 1, 4), 0m, 0m, 0m, 9m, 0m),
            new StockDataPoint(new DateTime(2024, 1, 5), 0m, 0m, 0m, 10m, 0m),
            new StockDataPoint(new DateTime(2024, 1, 6), 0m, 0m, 0m, 11m, 0m),
            new StockDataPoint(new DateTime(2024, 1, 7), 0m, 0m, 0m, 10m, 0m),
            new StockDataPoint(new DateTime(2024, 1, 8), 0m, 0m, 0m, 9m, 0m),
            new StockDataPoint(new DateTime(2024, 1, 9), 0m, 0m, 0m, 8m, 0m),
        };
        var smaResult = sma.Calculate(stockData);
        var emaResult = ema.Calculate(stockData);

        for (int i = 1; i < emaResult.Count - 1; i++)
        {
            var previousSMAValue = smaResult[i - 1].Value;
            var currentSMAVale = smaResult[i].Value;

            var previousEMAValue = emaResult[i - 1].Value;
            var currentEMAValue = emaResult[i].Value;
            
            if (previousEMAValue <= previousSMAValue 
                && currentEMAValue > currentSMAVale
               )
            {
                var emaCrossedOverSmaAt = emaResult[i].Timestamp;
                Assert.AreEqual(new DateTime(2024, 1, 4), emaCrossedOverSmaAt);
            }
        }
    }
    [TestMethod]
    public void Should_IdentifyAStockWithPositiveTrendAndLowRisk()
    {
        var sma = new SimpleMovingAverage(2, p => p.Close);
        var vol = new Volatility(2, p => p.Close);
        var allStocks = new Dictionary<string, List<StockDataPoint>>();
        
        var stableUp = new List<StockDataPoint>()
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 100m, 101m, 99m, 100m, 1000m),
            new StockDataPoint(new DateTime(2024, 1, 2), 101m, 102m, 100m, 101m, 1000m),
            new StockDataPoint(new DateTime(2024, 1, 3), 102m, 103m, 101m, 102m, 1000m),
            new StockDataPoint(new DateTime(2024, 1, 4), 103m, 104m, 102m, 103m, 1000m),
            new StockDataPoint(new DateTime(2024, 1, 5), 104m, 105m, 103m, 104m, 1000m),
            new StockDataPoint(new DateTime(2024, 1, 6), 105m, 106m, 104m, 105m, 1000m)
        };
        
        allStocks.Add("stableUp",stableUp);

        var volatileUp = new List<StockDataPoint>()
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 100m, 106m, 94m, 100m, 1200m),
            new StockDataPoint(new DateTime(2024, 1, 2), 104m, 110m, 98m, 104m, 1200m),
            new StockDataPoint(new DateTime(2024, 1, 3), 102m, 108m, 96m, 102m, 1200m),
            new StockDataPoint(new DateTime(2024, 1, 4), 107m, 113m, 101m, 107m, 1200m),
            new StockDataPoint(new DateTime(2024, 1, 5), 109m, 115m, 103m, 109m, 1200m),
            new StockDataPoint(new DateTime(2024, 1, 6), 112m, 118m, 106m, 112m, 1200m)
        };
        
        allStocks.Add("volatileUp",volatileUp);

        
        var flatLowRisk = new List<StockDataPoint>()
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 100m, 101m, 99m, 100m, 900m),
            new StockDataPoint(new DateTime(2024, 1, 2), 100m, 101m, 99m, 100m, 900m),
            new StockDataPoint(new DateTime(2024, 1, 3), 100m, 101m, 99m, 100m, 900m),
            new StockDataPoint(new DateTime(2024, 1, 4), 100m, 101m, 99m, 100m, 900m),
            new StockDataPoint(new DateTime(2024, 1, 5), 100m, 101m, 99m, 100m, 900m),
            new StockDataPoint(new DateTime(2024, 1, 6), 100m, 101m, 99m, 100m, 900m)
        };
        
        allStocks.Add("flatLowRisk",flatLowRisk);
        
        var downTrend = new List<StockDataPoint>()
        {
            new StockDataPoint(new DateTime(2024, 1, 1), 110m, 111m, 109m, 110m, 1100m),
            new StockDataPoint(new DateTime(2024, 1, 2), 109m, 110m, 108m, 109m, 1100m),
            new StockDataPoint(new DateTime(2024, 1, 3), 108m, 109m, 107m, 108m, 1100m),
            new StockDataPoint(new DateTime(2024, 1, 4), 107m, 108m, 106m, 107m, 1100m),
            new StockDataPoint(new DateTime(2024, 1, 5), 106m, 107m, 105m, 106m, 1100m),
            new StockDataPoint(new DateTime(2024, 1, 6), 105m, 106m, 104m, 105m, 1100m)
        };
        allStocks.Add("downTrend",downTrend);
        KeyValuePair<string, List<StockDataPoint>> bestStock = new KeyValuePair<string, List<StockDataPoint>>();

        decimal bestRisk = 9999999m;
        
        
        foreach (var stock in allStocks)
        {
            var smaValue = sma.Calculate(stock.Value);
            var volValue = vol.Calculate(stock.Value);

            var firstSmaValue = smaValue[0];
            var lastSmaValue = smaValue.Last();
            var lastVolValue = volValue[0].Value;

            if (lastSmaValue.Value > firstSmaValue.Value)
            {
                if (lastVolValue < bestRisk)
                {
                    bestRisk = lastVolValue;
                    bestStock = stock;
                }
            }
        }
        Assert.AreEqual("stableUp", bestStock.Key);
    }
}