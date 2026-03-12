using Microsoft.VisualStudio.TestTools.UnitTesting;
using LovelaceGroup6.src.StockAnalysis.Indicators;
using LovelaceGroup6.StockAnalysis.Models;
using System;
using System.Collections.Generic;
namespace LovelaceGroup6.Tests.Indicators
{
    [TestClass]
    public class ReturnCalculationTests
    {
        [TestMethod]
        public void Calculate_ReturnsCorrectReturnValues()
        {
            // Arrange (prepare test data)
            var data = new List<StockDataPoint>
            {
                new StockDataPoint(new DateTime(2024,1,1), 0,0,0,100,0),
                new StockDataPoint(new DateTime(2024,1,2), 0,0,0,110,0),
                new StockDataPoint(new DateTime(2024,1,3), 0,0,0,121,0)
            };

            var indicator = new ReturnCalculation();

            // Act (run the calculation)
            var result = indicator.Calculate(data);

            // Assert (check if result is correct)
            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(0.10m, result[0].Value);
            Assert.AreEqual(0.10m, result[1].Value);
        }
        [TestMethod]
        public void Calculate_ReturnsNegativeReturn_WhenPriceDrops()
        {
            // Arrange (create test data)
            var data = new List<StockDataPoint>
            {
                new StockDataPoint(new DateTime(2024,1,1), 0,0,0,100,0),
                new StockDataPoint(new DateTime(2024,1,2), 0,0,0,90,0)
            };

            var indicator = new ReturnCalculation();

            // Act (run calculation)
            var result = indicator.Calculate(data);

            // Assert (check the result)
            Assert.AreEqual(1, result.Count);
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

            var indicator = new ReturnCalculation();

            Action act = () => indicator.Calculate(data);

            Assert.ThrowsExactly<DivideByZeroException>(act);
        }
    }
}