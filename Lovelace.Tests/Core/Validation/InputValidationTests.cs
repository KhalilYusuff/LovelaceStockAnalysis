using Lovelace.StockAnalysis.Core.Validation;
using Lovelace.StockAnalysis.Indicators;
using Lovelace.StockAnalysis.Models;
using Lovelace.StockAnalysis.Tests.Utilities;

namespace Lovelace.Tests.Core.Validation
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

        [TestMethod]
        public void ValidateVolitilityPeriod_WithLessThanTwo_DoesThrow()
        {
            Action act = () => InputValidation.ValidateVolatilityPeriod(1);
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void ValidateVolatitilyPeriod_WithValidPeriod_DoesNotThrow()
        {
            InputValidation.ValidateVolatilityPeriod(3);
        }

        [TestMethod]
        public void ValidateSelector_DoesThrow()
        {
            Action act = () => InputValidation.ValidateSelector(null);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }

        [TestMethod]
        public void ValidateSelector_DoesNotThrow()
        {
            InputValidation.ValidateSelector(x => x.Open);
        }
    }
}