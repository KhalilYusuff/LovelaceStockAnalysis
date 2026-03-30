using Lovelace.StockAnalysis.Indicators;

namespace Lovelace.StockAnalysis.Tests.Indicators
{
    [TestClass]
    public class ReturnCalculationTests
    {
        [TestMethod]
        public void Calculate_ReturnsCorrectReturnValues()
        {
            var data = new List<StockDataPoint>
            {
                new StockDataPoint(new DateTime(2024,1,1), 0,0,0,100,0),
                new StockDataPoint(new DateTime(2024,1,2), 0,0,0,110,0),
                new StockDataPoint(new DateTime(2024,1,3), 0,0,0,121,0)
            };

            var indicator = new ReturnCalculation(x => x.Close);

            var result = indicator.Calculate(data);
            var count = result.Count;
            Assert.AreEqual(2, count);

            Assert.AreEqual(0.10m, result[0].Value);
            Assert.AreEqual(0.10m, result[1].Value);
        }
        [TestMethod]
        public void Calculate_ReturnsNegativeReturn_WhenPriceDrops()
        {
            var data = new List<StockDataPoint>
            {
                new StockDataPoint(new DateTime(2024,1,1), 0,0,0,100,0),
                new StockDataPoint(new DateTime(2024,1,2), 0,0,0,90,0)
            };

            var indicator = new ReturnCalculation(x => x.Close);

            var result = indicator.Calculate(data);
            var count = result.Count;
            Assert.AreEqual(1, count);
            Assert.AreEqual(-0.10m, result[0].Value);
        }
        
        [TestMethod]
        public void Calculate_ThrowsException_WhenPreviousCloseIsZero()
        {
            var data = new List<StockDataPoint>
            {
                new StockDataPoint(new DateTime(2024,1,1),0,0,0,0,0),
                new StockDataPoint(new DateTime(2024,1,2),0,0,0,100,0)
                
            };

            var indicator = new ReturnCalculation(x => x.Close);
            Action act = () => indicator.Calculate(data);

            Assert.ThrowsExactly<DivideByZeroException>(act);
        }
    }
}