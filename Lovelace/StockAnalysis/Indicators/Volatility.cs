using Lovelace.StockAnalysis.Core.Validation;
using Lovelace.StockAnalysis.Models;

namespace Lovelace.StockAnalysis.Indicators;


/// <summary>
/// Calculates the rolling standard deviation of stock prices over a specified period.
/// </summary>
/// <remarks>
/// This indicator uses sample standard deviation formula (denominator N-1) to calculate price volatility
/// over a sliding window. A higher value indicates greater price variability and thus higher
/// market volatility. The returned list contains one result per completed window,
/// starting at index <c> period - 1 </c> and ending at the last data point.
/// </remarks>
public sealed class Volatility : IIndicator
{
    private readonly int _period;
    private readonly Func<StockDataPoint, decimal> _selector;

    /// <summary>
    /// Initializes a new instance of the <see cref="Volatility"/> class.
    /// </summary>
    /// <param name="period">
    /// The number of data points to include in each rolling window. Must be at least 2.
    /// </param>
    /// <param name="selector">
    /// A function that selects the price value to use from each <see cref="StockDataPoint"/>,
    /// for example x => x.Close or x => x.High.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="period"/> is less than 2. The number of data points in each rolling window
    /// must be at least 2, since standard deviation measures the spread 
    /// between values and requires a minimum of two data points to be meaningful.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="selector"/> is null.
    /// </exception>
    public Volatility(int period, Func<StockDataPoint, decimal> selector)
    {
        if (period < 2) 
        {
            throw new ArgumentOutOfRangeException(nameof(period), "Period must be at least 2.");
        }


        if (selector == null)
        {

            throw new ArgumentNullException(nameof(selector), "A price selector function must be provided," +
                "for example: x => x.Close ");
        }

        _period = period;
        _selector = selector;

    }

    /// <summary>
    /// Calculates the rolling standard deviation of the selected price over the provided period.
    /// </summary>
    /// <param name="data">The stock data points to calculate volatility for.</param>
    /// <returns>
    /// A list of <see cref="IndicatorResult"/> values containing the timestamp
    /// and the calculated standard deviation for each window.
    /// </returns>

    public IReadOnlyList<IndicatorResult> Calculate(IReadOnlyList<StockDataPoint> data)
    {

        InputValidation.ValidateData(data);
        InputValidation.ValidatePeriod(_period, data.Count);

        var results = new List<IndicatorResult>(data.Count - _period + 1);

        for (int i = _period - 1; i < data.Count; i++)
        {
            decimal mean = 0;
            for (int j = i - _period + 1; j <= i; j++)

            {
                mean += _selector(data[j]);
            }

            mean /= _period;
            decimal sumSquaredDiffs = 0;

            for (int j = i - _period + 1; j <= i; j++)
            {
                decimal diff = _selector(data[j]) - mean;
                sumSquaredDiffs += diff * diff;
            }

            decimal variance = sumSquaredDiffs / (_period - 1);
            decimal standardDeviation = (decimal)Math.Sqrt((double)variance);

            results.Add(new IndicatorResult(data[i].Timestamp, standardDeviation));
        }

        return results.AsReadOnly();
    }
}
