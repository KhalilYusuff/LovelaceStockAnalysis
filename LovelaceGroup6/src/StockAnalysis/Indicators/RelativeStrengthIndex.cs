using LovelaceGroup6.src.StockAnalysis.Core.Validation;
using LovelaceGroup6.StockAnalysis.Interfaces;
using LovelaceGroup6.StockAnalysis.Models;

namespace LovelaceGroup6.src.StockAnalysis.Indicators;

/// <summary>
/// Calculates the Relative Strength Index (RSI) over a specified period.
/// </summary>
/// <remarks>
/// RSI is a momentum indicator producing values between 0 and 100.
/// Requires at least <c>period + 1</c> data points. Returns one result per data point
/// starting at index <c>period</c>.
/// </remarks>
public sealed class RelativeStrengthIndex : IMultiSeriesIndicator
{
    private readonly int _period;
    private readonly Func<StockDataPoint, decimal> _selector;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="period">Number of data points per RSI calculation. Must be greater than zero.</param>
    /// <param name="selector">Selects the price value from each data point.</param>
    /// <exception cref="ArgumentNullException">Thrown if selector is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if period is less than or equal to zero.</exception>
    public RelativeStrengthIndex(int period, Func<StockDataPoint, decimal> selector)
    {
        if (selector == null)
            throw new ArgumentNullException(nameof(selector));
        if (period <= 0)
            throw new ArgumentOutOfRangeException(nameof(period), "Period must be greater than zero.");

        _period = period;
        _selector = selector;
    }

    /// <summary>
    /// Calculates RSI values for the provided stock data series.
    /// </summary>
    /// <param name="series">
    /// A collection of stock data series where:
    /// index 0 = price data (required),
    /// index 1 = volume data (optional),
    /// index 2 = benchmark data (optional).
    /// Cannot be null or empty.
    /// </param>
    /// <returns>A read-only list of RSI results between 0 and 100.</returns>
    /// <exception cref="ArgumentNullException">Thrown if series is null.</exception>
    /// <exception cref="ArgumentException">Thrown if series is empty.</exception>
    public IReadOnlyList<IndicatorResult> Calculate(IReadOnlyList<IReadOnlyList<StockDataPoint>> series)
    {
        if (series == null)
            throw new ArgumentNullException(nameof(series));
        if (series.Count == 0)
            throw new ArgumentException("Series collection cannot be empty.", nameof(series));

        return Calculate(series[0]);
    }

    /// <summary>
    /// Calculates RSI values for a single series of stock data points.
    /// </summary>
    /// <param name="data">The collection of stock data points to process. Cannot be null, must be sorted
    /// chronologically, and contain enough entries for the configured period.</param>
    /// <returns>A read-only list of RSI results between 0 and 100.</returns>
    /// <exception cref="ArgumentNullException">Thrown if data is null.</exception>
    /// <exception cref="ArgumentException">Thrown if data is empty, unsorted, or too short.</exception>
    private IReadOnlyList<IndicatorResult> Calculate(IReadOnlyList<StockDataPoint> data)
    {
        // Validate input
        InputValidation.ValidateData(data);
        InputValidation.ValidatePeriod(_period, data.Count);
        if (data.Count < _period + 1)
            throw new ArgumentException(
                $"RSI requires at least {_period + 1} data points for period {_period}.",
                nameof(data));

        // RSI calculation logic (price changes, gains/losses, initial averages and Wilder's smoothing)
        // adapted from:
        // BT. (2021, March 16). A step-by-step guide for calculating reliable RSI values programmatically.
        // Medium. https://turmanaube.medium.com/a-step-by-step-guide-for-calculating-reliable-rsi-values-programmatically-a6a604a06b77

        // Calculate price changes and separate into gains and losses
        var gains = new List<decimal>();
        var losses = new List<decimal>();
        for (int i = 1; i < data.Count; i++)
        {
            decimal change = _selector(data[i]) - _selector(data[i - 1]);
            gains.Add(change > 0 ? change : 0);
            losses.Add(change < 0 ? Math.Abs(change) : 0);
        }

        // Calculate initial average gain and loss over the first period
        decimal avgGain = 0;
        decimal avgLoss = 0;
        for (int i = 0; i < _period; i++)
        {
            avgGain += gains[i];
            avgLoss += losses[i];
        }
        avgGain /= _period;
        avgLoss /= _period;

        // Calculate first RSI value
        var results = new List<IndicatorResult>();
        results.Add(new IndicatorResult(data[_period].Timestamp, CalculateRsi(avgGain, avgLoss)));

        // Apply Wilder's smoothing for remaining data points
        for (int i = _period + 1; i < data.Count; i++)
        {
            avgGain = (avgGain * (_period - 1) + gains[i - 1]) / _period;
            avgLoss = (avgLoss * (_period - 1) + losses[i - 1]) / _period;
            results.Add(new IndicatorResult(data[i].Timestamp, CalculateRsi(avgGain, avgLoss)));
        }

        return results.AsReadOnly();
    }

    private decimal CalculateRsi(decimal avgGain, decimal avgLoss)
    {
        if (avgLoss == 0) return 100;
        decimal rs = avgGain / avgLoss;
        return 100 - (100 / (1 + rs));
    }
}