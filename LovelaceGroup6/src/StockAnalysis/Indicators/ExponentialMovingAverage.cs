using LovelaceGroup6.src.StockAnalysis.Core.Validation;
using LovelaceGroup6.StockAnalysis.Interfaces;
using LovelaceGroup6.StockAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace LovelaceGroup6.src.StockAnalysis.Indicators
{   
    /// <summary>
    /// Represents an indicator that calculates the Exponential Moving Average (EMA) for a sequence of stock data points
    /// using a specified period and value selector function.
    /// </summary>
    /// <remarks>The ExponentialMovingAverage class uses a SimpleMovingAverage internally to assist with
    /// initial calculations before the exponential weighting is fully established. This indicator is commonly used in
    /// financial analysis to smooth out price data and identify trends by applying greater weight to more recent data
    /// points.</remarks>
    public class ExponentialMovingAverage : IIndicator
    {
        private readonly int _period;
        private readonly Func<StockDataPoint, decimal> _selector;
        private readonly SimpleMovingAverage _sma;

        /// <summary>
        /// Initializes a new instance of the ExponentialMovingAverage class with the specified period and value
        /// selector function.
        /// </summary>
        /// <remarks>The ExponentialMovingAverage class uses a SimpleMovingAverage internally to assist
        /// with initial calculations before the exponential weighting is fully established.</remarks>
        /// <param name="period">The number of periods to use when calculating the exponential moving average. Must be a positive integer.</param>
        /// <param name="selector">A function that extracts the decimal value from each StockDataPoint to be included in the moving average
        /// calculation. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when the selector function is null.</exception>
        public ExponentialMovingAverage(int period, Func<StockDataPoint, decimal> selector)
        {
            _period = period;
            _selector = selector ?? throw new ArgumentNullException("A valid selector must be passed on");
            _sma = new SimpleMovingAverage(period, selector);
        }

        /// <summary>
        /// Calculates the Exponential Moving Average (EMA) for a sequence of stock data points using the configured
        /// period and value selector.
        /// </summary>
        /// <remarks>The initial EMA value is calculated using the Simple Moving Average (SMA) of the
        /// first period's data points. Subsequent EMA values are computed recursively. The input data and period are
        /// validated before calculation.</remarks>
        /// <param name="data">The collection of stock data points to use for the EMA calculation. Must contain at least as many elements
        /// as the configured period.</param>
        /// <returns>A read-only list of IndicatorResult objects, each representing the EMA value and corresponding timestamp for
        /// each data point after the initial period.</returns>
        public IReadOnlyList<IndicatorResult> Calculate(IReadOnlyList<StockDataPoint> data)
        {
            InputValidation.ValidateData(data);
            InputValidation.ValidatePeriod(_period, data.Count);

            var results = new List<IndicatorResult>();
            
           
            decimal multiplier = 2m / (_period + 1);
            
            
            var smaResults = _sma.Calculate(data);
            decimal previousEma = smaResults[0].Value;
            
      
            results.Add(new IndicatorResult(data[_period - 1].Timestamp, previousEma));
            
            
            for (int i = _period; i < data.Count; i++)
            {
                decimal currentValue = _selector(data[i]);
                decimal ema = (currentValue * multiplier) + (previousEma * (1 - multiplier));
                results.Add(new IndicatorResult(data[i].Timestamp, ema));
                previousEma = ema;
            }
            
            return results;
        }
    }
}
