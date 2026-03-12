using Microsoft.VisualStudio.TestTools.UnitTesting;
using LovelaceGroup6.src.StockAnalysis.Core.Validation;
using LovelaceGroup6.StockAnalysis.Models;
using System;
using System.Collections.Generic;

namespace LovelaceGroup6.Tests.Indicators
{
    [TestClass]
    public class InputValidationTests
    {
        [TestMethod]
        public void ValidateData_WithNullData_ThrowsArgumentNullException()
        {
            Action act = () => InputValidation.ValidateData(null);
            Assert.ThrowsExactly<ArgumentNullException>(act);
        }

        [TestMethod]
        public void ValidateData_WithEmptyData_ThrowsArgumentException()
        {
            var empty = new List<StockDataPoint>();
            Action act = () => InputValidation.ValidateData(empty);
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void ValidateData_WithNonChronologicalData_ThrowsArgumentException()
        {
            var data = new List<StockDataPoint>
            {
                new StockDataPoint(new DateTime(2024, 1, 2), 0, 0, 0, 0, 0),
                new StockDataPoint(new DateTime(2024, 1, 1), 0, 0, 0, 0, 0)
            };
            Action act = () => InputValidation.ValidateData(data);
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void ValidatePeriod_WithZeroPeriod_ThrowsArgumentException()
        {
            Action act = () => InputValidation.ValidatePeriod(0, 10);
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void ValidatePeriod_WithPeriodExceedingDataCount_ThrowsArgumentException()
        {
            Action act = () => InputValidation.ValidatePeriod(15, 10);
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void ValidatePeriod_WithValidPeriod_DoesNotThrow()
        {
            InputValidation.ValidatePeriod(5, 10);
        }
    }
}