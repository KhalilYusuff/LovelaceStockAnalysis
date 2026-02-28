using LovelaceGroup6.StockAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LovelaceGroup6.src.StockAnalysis.Core.Validation
{
    /// <summary>
    /// Provides static methods for validating input data and parameters used in stock data analysis operations.
    /// </summary>
    /// <remarks>This class includes utility methods to ensure that input data collections and calculation
    /// parameters meet required preconditions before processing. All methods throw exceptions if validation fails. This
    /// class cannot be instantiated.</remarks>
    public static class InputValidation
    {
        /// <summary>
        /// Validates that the specified stock data collection is not null, not empty, and is ordered chronologically.
        /// </summary>
        /// <param name="data">The collection of stock data points to validate. Cannot be null or empty.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="data"/> is empty.</exception>
        public static void ValidateData(IReadOnlyList<StockDataPoint> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Count == 0)
            {
                throw new ArgumentException("Data cannot be empty");
            }

            EnsureChronologicalOrder(data);
        }

        /// <summary>
        /// Validates that the specified period is a positive integer and does not exceed the available data count.
        /// </summary>
        /// <param name="period">The period to validate. Must be greater than zero and less than or equal to <paramref name="dataCount"/>.</param>
        /// <param name="dataCount">The total number of available data points. Must be greater than or equal to <paramref name="period"/>.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="period"/> is less than or equal to zero, or if <paramref name="period"/> is
        /// greater than <paramref name="dataCount"/>.</exception>
        public static void ValidatePeriod(int period, int dataCount)
        {
            if (period <= 0)
            {
                throw new ArgumentException("Period must be greater than zero");
            }

            if (period > dataCount)
            {
                throw new ArgumentException("Period cannot exceed data count");
            }
        }

        private static void EnsureChronologicalOrder(IReadOnlyList<StockDataPoint> data)
        {
            for (int i = 1; i < data.Count; i++)
            {
                if (data[i].Timestamp < data[i - 1].Timestamp)
                {
                    throw new ArgumentException("Data must be sorted chronologically");
                }
            }
        }
    }
}
