using Lovelace.StockAnalysis.Core.Validation;
using Lovelace.StockAnalysis.Models;

namespace Lovelace.StockAnalysis.Indicators;

    /// <summary>
    /// Represents a simple moving average (SMA) indicator for time series data, such as stock prices.
    /// </summary>
    /// <remarks>The SimpleMovingAverage class calculates the arithmetic mean of a specified number of
    /// consecutive data points in a time series, producing a smoothed series that can be used to identify trends. The
    /// selector function determines which value from each StockDataPoint is used in the calculation (for example,
    /// closing price or volume). This class is typically used in financial analysis to smooth out short-term
    /// fluctuations and highlight longer-term trends in data.</remarks>
    public class SimpleMovingAverage : IIndicator
    {
        private readonly int _period;
        private readonly Func<StockDataPoint, decimal> _selector;

        /// <summary>
        /// Initializes a new instance of the SimpleMovingAverage class with the specified period and value selector.
        /// </summary>
        /// <param name="period">The number of data points to include in each moving average calculation. Must be greater than zero.</param>
        /// <param name="selector">A function that selects the decimal value from each StockDataPoint to be used in the moving average
        /// calculation. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if selector is null.</exception>
        public SimpleMovingAverage(int period, Func<StockDataPoint, decimal> selector)
        {
            _period = period;
            _selector = selector ?? throw new ArgumentNullException("A valid selector must be passed on");
        }

        /// <summary>
        /// Calculates the indicator values for the specified stock data using the configured period and selector.
        /// </summary>
        /// <param name="data">The collection of stock data points to process. Cannot be null and must contain enough elements for the
        /// configured period.</param>
        /// <returns>A read-only list of indicator results, each containing the timestamp and calculated value. The list will be
        /// empty if there are not enough data points to perform any calculations.</returns>
        public IReadOnlyList<IndicatorResult> Calculate(IReadOnlyList<StockDataPoint> data)
        {
            InputValidation.ValidateData(data);
            InputValidation.ValidatePeriod(_period, data.Count);

            var result = new List<IndicatorResult>(); 

            for (int i = 0; i <= data.Count - _period; i++)
            {
                decimal sum = 0;

                for (int j = 0; j < _period; j++)
                {
                    sum += _selector(data[i + j]);
                }

                var average = sum / _period;
                var timestamp = data[i + _period - 1].Timestamp;

                result.Add(new IndicatorResult(timestamp, average));

            }
            return result;
        }
    }

